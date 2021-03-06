﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

	[SerializeField] int m_MaxHealth = 3;
	[SyncVar (hook = "OnHealthChanged")] int m_Health;
	NetworkPlayer m_Player;
	public int MaxHealth{ get{ return m_MaxHealth; } }
	public int Health{ get{ return m_Health; } }

	public NetworkPlayer Player{ get{ return m_Player; }}

	void Awake(){
		m_Player = GetComponent<NetworkPlayer>();
	}

	[ServerCallback]
	void OnEnable(){
		m_Health = m_MaxHealth;
	}

	//[ServerCallback]
	void Start () {
		OnHealthChanged(m_MaxHealth);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Server]
	public bool TakeDamage(){
		bool died = false;
		if(m_Health <= 0) return false;

		m_Health--;
		died = m_Health <= 0;
		RpcTakeDamage(died);

		return died;
	}

	[ClientRpc]
	void RpcTakeDamage(bool died){
		if(isLocalPlayer){
			// Some ui flash effect for taking damage
			// PlayerUI.Instance.PlayerSomeEffect()
		}

		if(died){
			m_Player.Die();
		}
	}

	[Command]
	public void CmdAddHealth(int health){
		if(m_Health >= m_MaxHealth) return;

		if(m_Health + health >= m_MaxHealth)
			m_Health = m_MaxHealth;
		else
			m_Health += health;
	}

	void OnHealthChanged(int value){
		m_Health = value;
		if(isLocalPlayer)
			PlayerUI.Instance.SetHealth(value);
	}
}