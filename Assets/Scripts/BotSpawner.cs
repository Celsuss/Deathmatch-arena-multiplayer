using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BotSpawner : NetworkBehaviour {

	[SerializeField] GameObject m_BotPrefab;

	[ServerCallback]
	void Start(){
		GameObject obj = Instantiate(m_BotPrefab, transform.position, transform.rotation);
		obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
		obj.AddComponent<Bot>();
		NetworkServer.Spawn(obj);
	}
}
