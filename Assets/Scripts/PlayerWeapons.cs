using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapons : NetworkBehaviour {

	[SerializeField] Transform m_GunPivot;
	[SerializeField] GameObject[] m_Weapons;
	[SyncVar (hook="OnCurrentWeaponIndexChanged")] int m_CurrentWeaponIndex;
	GameObject m_CurrentWeapon;

	//[ServerCallback]
	void Start () {
		/*foreach(GameObject obj in m_Weapons){
			obj.SetActive(false);
			Debug.Log("Start");
		}*/

		if(m_Weapons.Length >= 1)
			OnCurrentWeaponIndexChanged(0);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer) return;

		if(Input.GetKeyDown("1") && m_CurrentWeaponIndex != 0 && m_Weapons.Length >= 1){
			CmdChangeWeapon(0);
		}
		else if(Input.GetKeyDown("2") && m_CurrentWeaponIndex != 1 && m_Weapons.Length >= 2){
			CmdChangeWeapon(1);
		}
	}

	[Command]
	void CmdChangeWeapon(int index){
		if(index >= m_Weapons.Length || index < 0) return;

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
	}
}