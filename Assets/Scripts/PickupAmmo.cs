using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : MonoBehaviour {

	[SerializeField] int m_Ammo = 90;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
        if(other.tag != "Player") return;

		other.GetComponent<PlayerShoot>().CmdAddAmmo(m_Ammo);
    }
}