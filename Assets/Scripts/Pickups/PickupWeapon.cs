using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupWeapon : PickupBase {
	[SerializeField] GameObject m_Weapon;

	protected override void Start () {
		base.Start();
		MeshFilter[] meshFilters = m_Weapon.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
	}
	
	protected override bool Apply (Collider other) {
		PlayerWeapons weapons = other.GetComponent<PlayerWeapons>();
		if(weapons.HoldingWeapon(m_Weapon))
			return false;

		weapons.CmdAddWeapon(m_Weapon);
		return true;
	}
}