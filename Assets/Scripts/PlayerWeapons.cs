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

	[Command]
	void CmdChangeWeapon(int index){
		if(index >= m_Weapons.Count || index < 0) return;

		m_CurrentWeaponIndex = index;
	}

	/*[Command]
	void CmdAddWeapon(int index){
		if(index >= m_Weapons.Length) return;

		Vector3 pos = m_GunPivot.transform.position;
		Quaternion rot = m_Weapons[index].transform.rotation * transform.rotation;
		GameObject obj = NetworkBehaviour.Instantiate(m_Weapons[index], pos, rot, m_GunPivot);
		NetworkServer.Spawn(obj);

		RpcProcessAddWeapon(obj, index);
		m_CurrentWeapon = obj;
		m_CurrentWeaponIndex = index;
	}

	[ClientRpc]
	void RpcProcessAddWeapon(GameObject obj, int index){
		obj.transform.position = m_GunPivot.transform.position;
		obj.transform.parent = m_GunPivot;
		m_CurrentWeapon = obj;
	}*/

	void OnCurrentWeaponIndexChanged(int value){
		m_CurrentWeaponIndex = value;

		if(m_CurrentWeapon)
			m_CurrentWeapon.SetActive(false);

		m_CurrentWeapon = m_Weapons[value];
		m_CurrentWeapon.SetActive(true);

		m_CurrentShotEffect = m_ShotEffects[value];
	}
}