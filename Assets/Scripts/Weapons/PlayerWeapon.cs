using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapon : NetworkBehaviour {

	/*[HideInInspector]*/ public bool BelongsToLocalPlayer = false;
	[SerializeField] float m_ShootCooldown = 0.3f;
	[SerializeField] float m_ReloadTime = 4f;
	[SerializeField] float m_Range = 50f;
	[SerializeField] int m_MaxAmmo = 90;
	[SerializeField] int m_MaxMagazine = 15;
	[SerializeField] string m_WeaponName;
	[SerializeField] [SyncVar (hook = "OnAmmoChanged")] int m_Ammo = 0;
	[SerializeField] [SyncVar (hook = "OnMagazineChanged")] int m_Magazine;

	public float ReloadTime{
		get{ return m_ReloadTime; }
	}
	public float ShootCooldown{
		get{ return m_ShootCooldown; }
	}
	public float Range{
		get{ return m_Range; }
	}
	public int MaxAmmo{
		get{ return m_MaxAmmo; }
	}
	public int MaxMagazine{
		get{ return m_MaxMagazine; }
	}
	public int Ammo{
		get{ return m_Ammo; }
		set{ OnAmmoChanged(value); }
	}
	public int Magazine{
		get{ return m_Magazine; }
		set{ OnMagazineChanged(value); }
	}
	public string WeaponName{
		get{ return m_WeaponName; }
	}

	//[ServerCallback]
	void Start () {
		OnAmmoChanged(m_MaxAmmo - m_MaxMagazine);
		OnMagazineChanged(m_MaxMagazine);
	}

	[ServerCallback]
	void OnEnable(){
		//Update UI
		OnAmmoChanged(m_Ammo);
		OnMagazineChanged(m_Magazine);
	}
	
	// Update is called once per frame
	void Update () {
		if(BelongsToLocalPlayer){
			PlayerUI.Instance.SetAmmo(m_Magazine, m_Ammo);
		}
	}

	[Command]
	public void CmdAddAmmo(int ammo){
		if(Ammo >= MaxAmmo) return;

		if(Ammo + ammo >= MaxAmmo)
			Ammo = MaxAmmo;
		else
			Ammo += ammo;
	}

	////
	/*[Command]
	public void CmdFireShot(Vector3 pos, Vector3 direction){
		Magazine--;
		Debug.Log("Setting magazine to: " + Magazine + " on weapon: " + WeaponName);
		
		RaycastHit hit;
		Ray ray = new Ray(pos, direction);
		Debug.DrawRay(pos, direction * 10f, Color.red, 1f);
		bool result = Physics.Raycast(ray, out hit, Range);
		
		if(result){
			PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
			if(enemy){
				if(enemy.TakeDamage()) {	// returns true if enemy died
					//m_Score++;
				}
			}
		}
		RpcProcessShotEffects(result, hit.point); 
	}

	[ClientRpc]
	void RpcProcessShotEffects(bool hit, Vector3 point){
		//m_PlayerWeapons.CurrenShotEffect.PlayShotEffects();
		//if(hit)
		//	m_PlayerWeapons.CurrenShotEffect.PlayImpactEffect(point);
	}*/
	////

	void OnAmmoChanged(int value){
		m_Ammo = value;
		if(BelongsToLocalPlayer){
			PlayerUI.Instance.SetAmmo(m_Magazine, value);
		}
	}

	void OnMagazineChanged(int value){
		m_Magazine = value;
		if(BelongsToLocalPlayer){
			PlayerUI.Instance.SetAmmo(value, m_Ammo);
		}
	}
}