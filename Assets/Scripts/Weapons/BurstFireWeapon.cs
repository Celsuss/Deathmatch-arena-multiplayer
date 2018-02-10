using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BurstFireWeapon : PlayerWeapon {

	[SerializeField] float m_RoundsPerBurst = 3;
	
	// Update is called once per frame
	protected override void Update () {
		if(!BelongsToLocalPlayer || m_ScoreManager.GameOver) return;

		m_ElapsedShootTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedShootTime > ShootCooldown && !m_Reloading && m_Magazine > 0){
			m_ElapsedShootTime = 0;
			CmdFireBurst();
		}

		if(Input.GetButtonDown("Reload") && !m_Reloading && m_Magazine < MaxMagazine){
			CmdStartReload();
		}
	}

	[Command]
	void CmdFireBurst(){
		StartCoroutine(BurstFire_Coroutine());
	}

	IEnumerator BurstFire_Coroutine(){
		for(int i = 0; i < m_RoundsPerBurst; ++i){
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
