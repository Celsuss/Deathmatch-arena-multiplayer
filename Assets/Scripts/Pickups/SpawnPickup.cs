using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPickup : NetworkBehaviour {

	[SerializeField] PickupBase m_Pickup;

	public override void OnStartServer(){
		GameObject obj = NetworkBehaviour.Instantiate(m_Pickup.gameObject, transform.position, m_Pickup.transform.rotation);
		NetworkServer.Spawn(obj);
		obj.SetActive(true);
	}
}
