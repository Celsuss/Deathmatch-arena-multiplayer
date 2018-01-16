using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapons : NetworkBehaviour {

	[SerializeField] Transform m_FirePosition;
	[SerializeField] Transform m_GunPivot;
	[SerializeField] PlayerWeapon m_StartingWeapon;
	[SerializeField] List<PlayerWeapon> m_Weapons;
	[SerializeField] List<ShotEffects> m_ShotEffects;
	[SerializeField] PlayerWeapon m_CurrentWeapon;
	[SerializeField] ShotEffects m_CurrentShotEffect;
	[SyncVar (hook="OnCurrentWeaponIndexChanged")] int m_CurrentWeaponIndex = 0;

	

	public PlayerWeapon CurrentWeapon{
		get { return m_Weapons[m_CurrentWeaponIndex]; }
	}
	public ShotEffects CurrenShotEffect{
		get { return m_CurrentShotEffect; }
	}

    public override void OnStartLocalPlayer(){
        
    }

    void Start () {
		if(isLocalPlayer)
			CmdAddStartingWeapon();
    }

	IEnumerator AddStartingWeapon_Coroutine(){
		// Hack because RPC methods dont work at the start.
		yield return new WaitForSeconds(0.3f);

		CmdAddWeapon(m_StartingWeapon.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer) return;

		for(int i = 0; i < m_Weapons.Count; i++){
			if(Input.GetKeyDown((i+1).ToString()) && m_CurrentWeaponIndex != i)
				CmdChangeWeapon(i);
		}
	}

	public bool HoldingWeapon(PlayerWeapon obj){
		foreach(PlayerWeapon weapon in m_Weapons){
			if(weapon.WeaponName.Equals(obj.WeaponName))
				return true;
		}
		return false;
	}

	[Command]
	void CmdChangeWeapon(int index){
		if(index >= m_Weapons.Count || index < 0)
			return;

		m_CurrentWeaponIndex = index;
	}

	[Command]
	public void CmdAddStartingWeapon(){
		StartCoroutine(AddStartingWeapon_Coroutine());
	}

	[Command]
	public void CmdAddWeapon(GameObject spawnObj){
		Vector3 pos = spawnObj.transform.position + m_GunPivot.transform.position;
		Quaternion rot = spawnObj.transform.rotation * m_GunPivot.transform.rotation;

        GameObject obj = NetworkBehaviour.Instantiate(spawnObj, spawnObj.transform.localPosition, rot, m_GunPivot);
		obj.transform.localPosition = spawnObj.transform.localPosition;
        NetworkServer.Spawn(obj);

		// Give ownership to player
		obj.GetComponent<NetworkIdentity>().AssignClientAuthority(gameObject.GetComponent<NetworkIdentity>().connectionToClient);

		RpcProcessAddWeapon(obj.transform.localPosition, obj.transform.localRotation, obj);
	}

	[ClientRpc]
	void RpcProcessAddWeapon(Vector3 localPos, Quaternion localRot, GameObject weapon){
        weapon.transform.parent = m_GunPivot;
        weapon.transform.localPosition = localPos;
        weapon.transform.localRotation = localRot;

		UpdateWeaponList(weapon);
	}

	void UpdateWeaponList(GameObject weapon){
		m_Weapons.Add(weapon.GetComponent<PlayerWeapon>());
		if(isLocalPlayer){
			m_Weapons[m_Weapons.Count-1].BelongsToLocalPlayer = true;
		}
		m_Weapons[m_Weapons.Count-1].FirePosition = m_FirePosition;
		
		m_ShotEffects.Add(weapon.GetComponentInChildren<ShotEffects>());

		if(m_CurrentWeapon != null)
			CmdChangeWeapon(m_Weapons.Count-1);
		else
			OnCurrentWeaponIndexChanged(0);
	}

	void OnCurrentWeaponIndexChanged(int value){
		if(m_Weapons.Count <= 0){
			PlayerWeapon obj = GetComponentInChildren<PlayerWeapon>();
			if(obj)
				UpdateWeaponList(obj.gameObject);
			return;
		}

		if(m_Weapons.Count <= value)
			return;

		m_CurrentWeaponIndex = value;

		if(m_CurrentWeapon){
			m_CurrentWeapon.gameObject.SetActive(false);
		}

		m_CurrentWeapon = m_Weapons[value];
		m_CurrentWeapon.gameObject.SetActive(true);
		m_CurrentShotEffect = m_ShotEffects[value];

		if(isLocalPlayer)
			UpdateUI();
	}

    void UpdateUI(){
		PlayerUI.Instance.SetAmmo(m_CurrentWeapon.Magazine, m_CurrentWeapon.Ammo);
	}
}