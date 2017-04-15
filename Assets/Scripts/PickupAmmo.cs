using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupAmmo : PickupBase {
	[SerializeField] int m_Ammo = 10;

	protected override bool AddPickupToPlayer (Collider other) {
		PlayerShoot shoot = other.GetComponent<PlayerShoot>();
		if(shoot.Ammo >= shoot.MaxAmmo)
			return false;

		shoot.CmdAddAmmo(m_Ammo);
		return true;
	}
}