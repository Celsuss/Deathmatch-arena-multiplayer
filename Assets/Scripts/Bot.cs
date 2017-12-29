using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bot : NetworkBehaviour {

	public bool m_CanShoot = true;

	[SerializeField] float m_ShotCooldown = 1f;

	PlayerShoot m_PlayerShoot;
	NetworkAnimator m_Anim;
	float m_EllapsedTime = 0f;

	// Use this for initialization
	void Start () {
		m_PlayerShoot = GetComponent<PlayerShoot>();
		m_Anim = GetComponent<NetworkAnimator>();
		GetComponent<NetworkPlayer>().m_PlayerName = "Bot";
	}
	
	// Update is called once per frame
	[ServerCallback]
	void Update () {
		// DEBUG STUFF

		m_Anim.animator.SetFloat("Forward", 0);
		//m_Anim.animator.SetFloat("Strafe", 0);

		if(Input.GetKey(KeyCode.Keypad8))
			m_Anim.animator.SetFloat("Forward", 1f);
		if(Input.GetKey(KeyCode.Keypad2))
			m_Anim.animator.SetFloat("Forward", -1f);

		/*if(Input.GetKey(KeyCode.Keypad4))
			m_Anim.animator.SetFloat("Strafe", -1f);
		if(Input.GetKey(KeyCode.Keypad6))
			m_Anim.animator.SetFloat("Strafe", 1f);*/
		
		/*if(Input.GetKey(KeyCode.Keypad7))
			m_Anim.animator.SetFloat("Died", 1f);
		if(Input.GetKey(KeyCode.Keypad9))
			m_Anim.animator.SetFloat("Restart", -1f);*/

		//BotAutoFire();
	}

	[Server]
	void BotAutoFire(){
		m_EllapsedTime += Time.deltaTime;
		if(m_EllapsedTime < m_ShotCooldown)
			return;

		m_EllapsedTime = 0f;
		/*if(m_PlayerShoot.enabled)
			m_PlayerShoot.FireAsBot();*/
	}
}
