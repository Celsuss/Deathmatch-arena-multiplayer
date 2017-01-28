using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

	[SerializeField] float m_ShootCooldown = 0.3f;
	[SerializeField] Transform m_FirePosition;
	[SerializeField] float m_Range = 50f;
	[SyncVar (hook = "OnScoreChange")] int m_Score;
	float m_ElapsedTime = 0;
	bool m_CanShoot;

	// Use this for initialization
	void Start () {
		if(isLocalPlayer)
			m_CanShoot = true;
		else
			m_CanShoot = false;
	}

	[ServerCallback]
	void OnEnable(){
		m_Score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_CanShoot) return;

		m_ElapsedTime += Time.deltaTime;
		if(Input.GetButtonDown("Fire1") && m_ElapsedTime > m_ShootCooldown){
			m_ElapsedTime = 0;
			CmdFireShot(m_FirePosition.position, m_FirePosition.forward);
		}
	}

	[Command]
	void CmdFireShot(Vector3 pos, Vector3 direction){
		RaycastHit hit;
		Ray ray = new Ray(pos, direction);
		Debug.DrawRay(pos, direction * 3f, Color.red, 1f);
		bool result = Physics.Raycast(ray, out hit, m_Range);
		
		if(result){
			PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
			if(enemy){
				if(enemy.TakeDamage()) {	//TakeDamage() returns true if enemy died
					m_Score++;
				}
			}
		}
		RpcProcessShotEffects(result, hit.point); 
	}

	[ClientRpc]
	void RpcProcessShotEffects(bool hit, Vector3 point){
		if(hit){
			//Add hit particle effect at point and play sound at point
		}
	}

	void OnScoreChange(int value){
		m_Score = value;
		if(isLocalPlayer){
			PlayerUI.Instance.SetKills(value);
		}
	}
}
