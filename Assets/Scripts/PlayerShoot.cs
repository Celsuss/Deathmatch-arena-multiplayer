using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {
	[SerializeField] AudioSource m_AudioSource;
	[SerializeField] AudioClip m_ReloadClip;
	[SerializeField] Transform m_FirePosition;
	[SerializeField] ShotEffects m_ShotEffects;
	[SerializeField] float m_ShootCooldown = 0.3f;
	[SerializeField] float m_ReloadTime = 4f;
	[SerializeField] float m_Range = 50f;
	[SerializeField] int m_MaxAmmo = 90;
	[SerializeField] int m_MaxMagazine = 15;
	[SyncVar (hook = "OnAmmoChanged")] int m_Ammo = 0;
	[SyncVar (hook = "OnMagazineChanged")] int m_Magazine;
	[SyncVar (hook = "OnScoreChanged")] int m_Score;
	float m_ElapsedShootTime = 0f;
	float m_ElapsedReloadTime = 0f;
	bool m_Reloading = false;
	bool m_CanShoot;

	// Use this for initialization
	void Start () {
		m_ShotEffects.Initialize();
		OnScoreChanged(m_Score);
		OnMagazineChanged(m_Magazine);
		m_Reloading = false;

		if(isLocalPlayer)
			m_CanShoot = true;
		else
			m_CanShoot = false;
	}

	[ServerCallback]
	void OnEnable(){
		m_Score = 0;
		m_Ammo = m_MaxAmmo - m_MaxMagazine;
		m_Magazine = m_MaxMagazine;
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_CanShoot) return;

		m_ElapsedShootTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedShootTime > m_ShootCooldown && !m_Reloading && m_Magazine > 0){
			m_ElapsedShootTime = 0;
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
		}

		if(Input.GetButtonDown("Reload") && !m_Reloading && m_Magazine < m_MaxMagazine){
			m_Reloading = true;
			m_ElapsedReloadTime = 0;
			CmdStartReload();
		}
		Reload();
	}

	void Reload(){
		if(m_Reloading){
			m_ElapsedReloadTime += Time.deltaTime;
			if(m_ElapsedReloadTime > m_ReloadTime)
				CmdFinishReload();
		}
	}

	[Command]
	void CmdStartReload(){
		//TODO: Reload animation
		RpcProcessReloadEffect();
	}

	[Command]
	void CmdFinishReload(){
		m_Reloading = false;

		if(m_Ammo >= m_MaxMagazine){
			m_Ammo -= m_MaxMagazine - m_Magazine;
			m_Magazine = m_MaxMagazine;
		}
		else{
			m_Magazine = m_Ammo;
			m_Ammo = 0;
		}
	}

	[Command]
	void CmdFireShot(Vector3 pos, Vector3 direction){
		m_Magazine--;
		RaycastHit hit;
		Ray ray = new Ray(pos, direction);
		Debug.DrawRay(pos, direction * 10f, Color.red, 1f);
		bool result = Physics.Raycast(ray, out hit, m_Range);
		
		if(result){
			PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
			if(enemy){
				if(enemy.TakeDamage()) {	//TakeDamage() returns true if enemy died
					m_Score++;
				}
			}
		}
		RpcProcessShotEffects(result, hit.point); 
	}

	[ClientRpc]
	void RpcProcessReloadEffect(){
		m_AudioSource.clip = m_ReloadClip;
		m_AudioSource.Play();
	}

	[ClientRpc]
	void RpcProcessShotEffects(bool hit, Vector3 point){
		m_ShotEffects.PlayShotEffects();
		if(hit)
			m_ShotEffects.PlayImpactEffect(point);
	}

	void OnScoreChanged(int value){
		m_Score = value;
		if(isLocalPlayer)
			PlayerUI.Instance.SetKills(value);
	}

	void OnAmmoChanged(int value){
		m_Ammo = value;
		if(isLocalPlayer)
			PlayerUI.Instance.SetAmmo(value, m_Ammo);
	}

	void OnMagazineChanged(int value){
		m_Magazine = value;
		if(isLocalPlayer)
			PlayerUI.Instance.SetAmmo(value, m_Ammo);
	}

	public void FireAsBot(){
		CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
	}
}
