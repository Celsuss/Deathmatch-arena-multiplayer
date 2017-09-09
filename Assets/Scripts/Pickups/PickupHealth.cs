using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupHealth : PickupBase {
	[SerializeField] int m_Health = 1;

	[Command]
	protected override void CmdApply (GameObject other) {
		PlayerHealth health = other.GetComponent<PlayerHealth>();
		if(health.Health >= health.MaxHealth)
			return;

		health.CmdAddHealth(m_Health);
		CmdFinishPickup();
	}
}