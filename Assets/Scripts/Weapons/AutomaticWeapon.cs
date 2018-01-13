using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutomaticWeapon : PlayerWeapon {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	protected override void Update () {
		if(!BelongsToLocalPlayer) return;

		m_ElapsedShootTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedShootTime > ShootCooldown && !m_Reloading && m_Magazine > 0){
			m_ElapsedShootTime = 0;
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
		}

		if(Input.GetButtonDown("Reload") && !m_Reloading && m_Magazine < MaxMagazine){
			CmdStartReload();
		}
	}

	
}