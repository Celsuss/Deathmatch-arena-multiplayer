using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InitializeParent : NetworkBehaviour {
	NetworkInstanceId ParentNetId;

	[SyncVar] public uint ParentNetIdValue;
	
	public override void OnStartClient(){
		ParentNetId = new NetworkInstanceId(ParentNetIdValue);
		Debug.Log("ParentNetId is " + ParentNetId.ToString() + ", value: " + ParentNetIdValue.ToString());

		GameObject parentObject = ClientScene.FindLocalObject(ParentNetId);
		transform.SetParent(parentObject.transform);
	}
}