using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPickup : NetworkBehaviour {

	[SerializeField] PickupBase m_Pickup;

	// Use this for initialization
	void Start () {
		
	}

	public override void OnStartServer(){
		GameObject obj = NetworkBehaviour.Instantiate(m_Pickup.gameObject, transform.position, m_Pickup.transform.rotation);
		NetworkServer.Spawn(obj);
		obj.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
