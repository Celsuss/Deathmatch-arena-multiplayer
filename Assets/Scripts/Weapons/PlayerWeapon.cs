using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapon : NetworkBehaviour {

	[SerializeField] AudioClip m_ShootClip;
	[SerializeField] AudioClip m_ReloadClip;
	ShotEffects m_ShotEffects;
	AudioSource m_AudioSource;
	/*[HideInInspector]*/ public bool BelongsToLocalPlayer = false;
	[SerializeField] protected float m_ShootCooldown = 0.3f;
	[SerializeField] protected float m_ReloadTime = 4f;
	[SerializeField] protected float m_Range = 50f;
	[SerializeField] protected int m_MaxAmmo = 90;
	[SerializeField] protected int m_MaxMagazine = 15;
	[SerializeField] string m_WeaponName;
	[SerializeField] [SyncVar (hook = "OnAmmoChanged")] int m_Ammo = 0;
	[SerializeField] [SyncVar (hook = "OnMagazineChanged")] protected int m_Magazine;
	[SyncVar] protected bool m_Reloading = false;
	protected Transform m_FirePosition;
	protected float m_ElapsedShootTime = 0f;
	ScoreManager m_ScoreManager;
	string m_PlayerName;

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
		m_ShotEffects = GetComponentInChildren<ShotEffects>();
		OnAmmoChanged(m_MaxAmmo - m_MaxMagazine);
		OnMagazineChanged(m_MaxMagazine);

		NetworkPlayer player = GetComponentInParent<NetworkPlayer>();
		m_PlayerName = player.m_PlayerName;

		FindScoreManager();
    }

	[ServerCallback]
	void OnEnable(){
		AudioSource audio = transform.GetComponentInParent<AudioSource>();
		m_AudioSource = audio;

		//Update UI
		OnAmmoChanged(m_Ammo);
		OnMagazineChanged(m_Magazine);
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(!BelongsToLocalPlayer) return;

		m_ElapsedShootTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedShootTime > ShootCooldown && !m_Reloading && m_Magazine > 0){
			m_ElapsedShootTime = 0;
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
		}

		if(Input.GetButtonDown("Reload") && !m_Reloading && m_Magazine < MaxMagazine){
			CmdStartReload();
		}
	}

	void FindScoreManager(){
		GameObject obj = GameObject.Find("Manager(Clone)");
		if(!obj)
			return;

		m_ScoreManager = obj.GetComponent<ScoreManager>();
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
	protected void CmdStartReload(){
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
	public virtual void CmdFireShot(Vector3 pos, Vector3 direction){
		Magazine--;
		
		RaycastHit hit;
		Ray ray = new Ray(pos, direction);
		Debug.DrawRay(pos, direction * 10f, Color.red, 1f);
		bool result = Physics.Raycast(ray, out hit, Range);
		
		if(result){
			PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
			if(enemy){
				if(enemy.TakeDamage()) {	// returns true if enemy died
					m_ScoreManager.CmdIncrementScore(m_PlayerName);
					m_ScoreManager.CmdIncrementDeaths(enemy.Player.name);
				}
			}
		}
		RpcProcessShotEffects(result, hit.point); 
	}

	[ClientRpc]
	void RpcProcessShotEffects(bool hit, Vector3 point){
		m_ShotEffects.PlayShotEffects();
	}
	
	[ClientRpc]
	void RpcProcessReloadEffect(){
		m_AudioSource.clip = m_ReloadClip;
		m_AudioSource.Play();
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