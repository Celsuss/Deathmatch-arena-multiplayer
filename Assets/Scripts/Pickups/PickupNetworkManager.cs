using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupNetworkManager : NetworkBehaviour {

	[SerializeField] List<Transform> Spawnpoints;
	[SerializeField] List<GameObject> Pickups;

	public override void OnStartServer(){
		// for(int i = 0; i < Pickups.Count; i++){
		// 	if(Spawnpoints.Count >= i){
		// 		GameObject obj = Pickups[i];
		// 		obj = NetworkBehaviour.Instantiate(obj, Spawnpoints[i].position, Pickups[i].transform.rotation);
		// 		NetworkServer.Spawn(obj);
		// 	}
		// }
	}
}	