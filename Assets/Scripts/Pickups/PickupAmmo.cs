using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupAmmo : PickupBase {
	[SerializeField] int m_Ammo = 10;

	[Command]
	protected override void CmdApply (GameObject other) {
		/*PlayerShoot shoot = other.GetComponent<PlayerShoot>();
		if(shoot.Ammo >= shoot.MaxAmmo)
			return false;

		shoot.CmdAddAmmo(m_Ammo);
		return true;*/

		PlayerWeapon weapon = other.GetComponent<PlayerWeapons>().CurrentWeapon;
		if(weapon.Ammo >= weapon.MaxAmmo)
			return;

		weapon.CmdAddAmmo(m_Ammo);
		CmdFinishPickup();
	}
}