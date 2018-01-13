using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupWeapon : PickupBase {

	[SerializeField] PlayerWeapon m_Weapon;

	protected override void Start () {
		MeshFilter[] meshFilters = m_Weapon.GetComponentsInChildren<MeshFilter>();
		if(meshFilters.Length > 0){
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			int i = 0;
			while (i < meshFilters.Length) {
				combine[i].mesh = meshFilters[i].sharedMesh;
				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
				i++;
			}

			if(GetComponent<MeshFilter>()){
				GetComponent<MeshFilter>().mesh = new Mesh();
				GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
			}
		}

		base.Start();
	}
	
	[Command]
	protected override void CmdApply (GameObject other) {
		PlayerWeapons weapons = other.GetComponent<PlayerWeapons>();
		if(weapons.HoldingWeapon(m_Weapon))
			return;

		weapons.CmdAddWeapon(m_Weapon.gameObject);
		CmdFinishPickup();
	}

	IEnumerator WaitForComponentToLoad_Coroutine(Collider other){
		yield return new WaitForSeconds(0.5f);
 		OnTriggerEnter (other);
	}
}