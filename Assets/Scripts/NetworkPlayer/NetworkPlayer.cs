﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool>{}

public class NetworkPlayer : NetworkBehaviour {

	[SyncVar (hook="OnNameChanged")] public string m_PlayerName;
	[SyncVar (hook="OnColorChanged")] public Color m_PlayerColor;
	[SyncVar] public bool m_IsPlayerLoaded = false;

	[SerializeField] ToggleEvent m_OnToggleShared;
	[SerializeField] ToggleEvent m_OnToggleLocal;
	[SerializeField] ToggleEvent m_OnToggleRemote;
	[SerializeField] float m_RespawnTime = 1f;
	GameObject m_MainCamera;
	NetworkAnimator m_Anim;

	// Use this for initialization
	void Start () {
		m_Anim = GetComponent<NetworkAnimator>();
		m_MainCamera = Camera.main.gameObject;
		EnablePlayer();

		//Calling hook methods because hook is not called when object is initializing
		InitializeHooks();
	}

	void InitializeHooks(){
		OnNameChanged(m_PlayerName);
		OnColorChanged(m_PlayerColor);
	}

	void EnablePlayer(){
		if(isLocalPlayer){
			PlayerUI.Instance.Initialize();
			m_MainCamera.SetActive(false);
		}

		m_OnToggleShared.Invoke(true);

		if(isLocalPlayer){
			m_OnToggleLocal.Invoke(true);
		}
		else{
			m_OnToggleRemote.Invoke(true);
		}
	}
	
	void DisablePlayer(){
		if(isLocalPlayer){
			PlayerUI.Instance.HideCrosshair();
			m_MainCamera.SetActive(true);
		}

		m_OnToggleShared.Invoke(false);

		if(isLocalPlayer){
			m_OnToggleLocal.Invoke(false);
		}
		else{
			m_OnToggleRemote.Invoke(false);
		}
	}

	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer) return;

		m_Anim.animator.SetFloat("Forward", Input.GetAxis("Vertical"));
		//m_Anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
	}

	public void Die(){
		if(isLocalPlayer){
			PlayerUI.Instance.WriteGameStatusText("You died");
			PlayerUI.Instance.PlayDeathAudio();

			m_Anim.SetTrigger("Died");
		}

		DisablePlayer();
		Invoke("Respawn", m_RespawnTime);
	}

	void Respawn(){
		if(isLocalPlayer){
			Transform spawn = NetworkManager.singleton.GetStartPosition();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;	
		}
		EnablePlayer();
	}

	void OnNameChanged(string value){
		m_PlayerName = value;
		gameObject.name = value;
		GetComponentInChildren<Text>(true).text = value;
	}

	void OnColorChanged(Color value){
		m_PlayerColor = value;
		GetComponentInChildren<RendererToggler>().ChangeColor(value);
	}
}
