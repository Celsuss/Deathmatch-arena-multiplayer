using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapons : NetworkBehaviour {

	[SerializeField] Transform m_GunPivot;
	[SerializeField] List<GameObject> m_Weapons;
	[SerializeField] List<ShotEffects> m_ShotEffects;
	[SyncVar (hook="OnCurrentWeaponIndexChanged")] int m_CurrentWeaponIndex;
	GameObject m_CurrentWeapon;
	ShotEffects m_CurrentShotEffect;
	public GameObject CurrentWeapon{
		get { return m_CurrentWeapon; }
	}
	public ShotEffects CurrenShotEffect{
		get { return m_CurrentShotEffect; }
	}

	//[ServerCallback]
	void Start () {
		foreach(GameObject obj in m_Weapons){
			//obj.SetActive(false);
			m_ShotEffects.Add(obj.GetComponentInChildren<ShotEffects>(true));
		}

		if(m_Weapons.Count >= 1)
			OnCurrentWeaponIndexChanged(0);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer) return;

		for(int i = 0; i < m_Weapons.Count; i++){
			if(Input.GetKeyDown((i+1).ToString()) && m_CurrentWeaponIndex != i)
				CmdChangeWeapon(i);
		}
	}

	public bool HoldingWeapon(GameObject obj){
		foreach(GameObject w in m_Weapons){
			if(obj.name.Equals(w.name))
				return true;
		}
		return false;
	}

	[Command]
	void CmdChangeWeapon(int index){
		if(index >= m_Weapons.Count || index < 0) return;

		m_CurrentWeaponIndex = index;
	}

	[Command]
	public void CmdAddWeapon(GameObject spawnObj){
		Vector3 pos = spawnObj.transform.position + m_GunPivot.transform.position;
		Quaternion rot = spawnObj.transform.rotation * m_GunPivot.transform.rotation;

		GameObject obj = NetworkBehaviour.Instantiate(spawnObj, pos, rot, m_GunPivot);
		obj.transform.localPosition = spawnObj.transform.localPosition;

		NetworkServer.Spawn(obj);
		RpcProcessAddWeapon(obj.transform.localPosition, obj.transform.localRotation, obj, obj.transform.parent.gameObject);
	}

	[ClientRpc]
	void RpcProcessAddWeapon(Vector3 localPos, Quaternion localRot, GameObject weapon, GameObject parent){
		weapon.transform.parent = m_GunPivot;
        weapon.transform.localPosition = localPos;
        weapon.transform.localRotation = localRot;

		UpdateWeaponList(weapon);
	}

	void UpdateWeaponList(GameObject weapon){
		m_Weapons.Add(weapon);
		m_ShotEffects.Add(weapon.GetComponentInChildren<ShotEffects>());

		CmdChangeWeapon(m_Weapons.Count-1);
	}

	void OnCurrentWeaponIndexChanged(int value){
		if(m_Weapons.Count <= value){
			Debug.Log("Failed to change current weapon to " + value);
			return;
		}

		m_CurrentWeaponIndex = value;

		if(m_CurrentWeapon)
			m_CurrentWeapon.SetActive(false);
		//if(m_Weapons.Count <= value)
		//	if(!FindNewWeapon()) return;


		m_CurrentWeapon = m_Weapons[value];
		m_CurrentWeapon.SetActive(true);

		m_CurrentShotEffect = m_ShotEffects[value];
	}

	bool FindNewWeapon(){
		

		return false;
	}
}