using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {
	[SerializeField] AudioSource m_AudioSource;
	[SerializeField] AudioClip m_ReloadClip;
	[SerializeField] Transform m_FirePosition;
	[SerializeField] PlayerWeapons m_PlayerWeapons;
	[SyncVar (hook = "OnScoreChanged")] int m_Score;
	[SyncVar] bool m_Reloading = false;
	float m_ElapsedShootTime = 0f;
	bool m_CanShoot;

	// Use this for initialization
	void Start () {
		OnScoreChanged(m_Score);
		m_PlayerWeapons = GetComponent<PlayerWeapons>();
		m_Reloading = false;

		if(isLocalPlayer)
			m_CanShoot = true;
		else
			m_CanShoot = false;
	}

	[ServerCallback]
	void OnEnable(){
		m_Score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_CanShoot) return;

		m_ElapsedShootTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedShootTime > m_PlayerWeapons.CurrentWeapon.ShootCooldown && !m_Reloading && m_PlayerWeapons.CurrentWeapon.Magazine > 0){
			m_ElapsedShootTime = 0;
			//m_PlayerWeapons.CurrentWeapon.CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
		}

		if(Input.GetButtonDown("Reload") && !m_Reloading && m_PlayerWeapons.CurrentWeapon.Magazine < m_PlayerWeapons.CurrentWeapon.MaxMagazine){
			CmdStartReload();
		}
	}
	IEnumerator Reload_Coroutine(){
		m_Reloading = true;
		yield return new WaitForSeconds(m_PlayerWeapons.CurrentWeapon.ReloadTime);

		if(m_PlayerWeapons.CurrentWeapon.Ammo >= m_PlayerWeapons.CurrentWeapon.MaxMagazine){
			m_PlayerWeapons.CurrentWeapon.Ammo -= m_PlayerWeapons.CurrentWeapon.MaxMagazine - m_PlayerWeapons.CurrentWeapon.Magazine;
			m_PlayerWeapons.CurrentWeapon.Magazine = m_PlayerWeapons.CurrentWeapon.MaxMagazine;
		}
		else{
			m_PlayerWeapons.CurrentWeapon.Magazine = m_PlayerWeapons.CurrentWeapon.Ammo;
			m_PlayerWeapons.CurrentWeapon.Ammo = 0;
		}

		m_Reloading = false;
	}

	[Command]
	void CmdStartReload(){
		//TODO: Reload animation
		StartCoroutine(Reload_Coroutine());
		RpcProcessReloadEffect();
	}

	[Command]
	void CmdFireShot(Vector3 pos, Vector3 direction){
		m_PlayerWeapons.CurrentWeapon.Magazine--;
		
		RaycastHit hit;
		Ray ray = new Ray(pos, direction);
		Debug.DrawRay(pos, direction * m_PlayerWeapons.CurrentWeapon.Range, Color.red, 1f);
		bool result = Physics.Raycast(ray, out hit, m_PlayerWeapons.CurrentWeapon.Range);
		
		if(result){
			PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
			if(enemy){
				if(enemy.TakeDamage()) {	// returns true if enemy died
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
		m_PlayerWeapons.CurrenShotEffect.PlayShotEffects();
		if(hit)
			m_PlayerWeapons.CurrenShotEffect.PlayImpactEffect(point);
	}

	void OnScoreChanged(int value){
		m_Score = value;
		if(isLocalPlayer)
			PlayerUI.Instance.SetKills(value);
	}

	public void FireAsBot(){
		//CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
	}
}