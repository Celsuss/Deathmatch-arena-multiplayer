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
	[SyncVar] bool m_Reloading = false;
	Transform m_FirePosition;
	float m_ElapsedShootTime = 0f;

	public Transform FirePosition{
		get { return m_FirePosition; }
		set { m_FirePosition = value; }
	}

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
		if(!BelongsToLocalPlayer) return;

		m_ElapsedShootTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedShootTime > ShootCooldown && !m_Reloading && m_Magazine > 0){
			m_ElapsedShootTime = 0;
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
		}

		if(Input.GetButtonDown("Reload") && !m_Reloading && m_Magazine < MaxMagazine){
			CmdStartReload();
		}

		/*if(BelongsToLocalPlayer){
			PlayerUI.Instance.SetAmmo(m_Magazine, m_Ammo);
		}*/
	}

	IEnumerator Reload_Coroutine(){
		m_Reloading = true;
		yield return new WaitForSeconds(m_ReloadTime);

		if(m_Ammo >= MaxMagazine){
			m_Ammo -= MaxMagazine - m_Magazine;
			m_Magazine = MaxMagazine;
		}
		else{
			m_Magazine = m_Ammo;
			m_Ammo = 0;
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
	public void CmdAddAmmo(int ammo){
		if(Ammo >= MaxAmmo) return;

		if(Ammo + ammo >= MaxAmmo)
			Ammo = MaxAmmo;
		else
			Ammo += ammo;
	}

	[Command]
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
	}
	
	[ClientRpc]
	void RpcProcessReloadEffect(){
		//m_AudioSource.clip = m_ReloadClip;
		//m_AudioSource.Play();
	}

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