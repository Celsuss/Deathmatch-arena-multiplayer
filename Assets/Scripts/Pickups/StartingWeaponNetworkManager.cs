using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StartingWeaponNetworkManager : NetworkBehaviour {

	[SerializeField] List<Transform> Spawnpoints;
	[SerializeField] GameObject StartingWeaponPickup;

	public override void OnStartServer(){
		for(int i = 0; i < Spawnpoints.Count; i++){
			if(Spawnpoints.Count >= i){
				GameObject obj = StartingWeaponPickup;
				obj = NetworkBehaviour.Instantiate(obj, Spawnpoints[i].position, StartingWeaponPickup.transform.rotation);
				NetworkServer.Spawn(obj);
			}
		}
	}
}