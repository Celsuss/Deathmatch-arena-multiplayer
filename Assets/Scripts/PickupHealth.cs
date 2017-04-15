using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupHealth : PickupBase {
	[SerializeField] int m_Health = 1;

	protected override bool AddPickupToPlayer (Collider other) {
		PlayerHealth health = other.GetComponent<PlayerHealth>();
		if(health.Health >= health.MaxHealth)
			return false;

		health.CmdAddHealth(m_Health);
		return true;
	}
}