using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkObjectsSpawner : NetworkBehaviour {

	[SerializeField] List<GameObject> m_SpawnList;
	
	public override void OnStartServer(){
		for(int i = 0; i < m_SpawnList.Count; i++){
			GameObject obj = m_SpawnList[i];
			obj = NetworkBehaviour.Instantiate(obj);
			NetworkServer.Spawn(obj);
		}
	}
}