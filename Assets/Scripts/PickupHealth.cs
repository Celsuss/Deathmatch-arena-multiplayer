using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : MonoBehaviour {

	[SerializeField] int m_Health = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
        if(other.tag != "Player") return;

		other.GetComponent<PlayerHealth>().CmdGiveHealth(m_Health);
    }
}