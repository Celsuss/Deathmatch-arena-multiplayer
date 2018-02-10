using System.Collections;
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
	ScoreManager m_ScoreManager;

	// Use this for initialization
	void Start () {
		m_Anim = GetComponent<NetworkAnimator>();
		m_MainCamera = Camera.main.gameObject;
		EnablePlayer();

		//Calling hook methods because hook is not called when object is initializing
		InitializeHooks();

		if(isServer)
			StartCoroutine(AddPlayerToScoreManager_Coroutine());

		FindScoreManager();
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

	// void OnPlayerDisconnected(NetworkPlayer player) {
    //     Debug.Log("Clean up after player " + player);
    //     //Network.RemoveRPCs(player);
    //     //Network.DestroyPlayerObjects(player);
    // }

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

	//[ServerCallback]
	void OnDestroy() {
		if(!m_ScoreManager)
			FindScoreManager();

		if(m_ScoreManager && !m_ScoreManager.GameOver)
			m_ScoreManager.CmdRemovePlayer(m_PlayerName);
    }

	void Respawn(){
		if(isLocalPlayer){
			Transform spawn = NetworkManager.singleton.GetStartPosition();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;	
		}
		EnablePlayer();
	}

	// Hack, keep looping until Manager(Clone) is instantiated.
	IEnumerator AddPlayerToScoreManager_Coroutine(){
		GameObject obj = GameObject.Find("Manager(Clone)");
		while(!obj){
			yield return new WaitForEndOfFrame();
			obj = GameObject.Find("Manager(Clone)");
		}
		
		ScoreManager score = obj.GetComponent<ScoreManager>();
		score.CmdAddPlayer(m_PlayerName);
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

	void FindScoreManager(){
		GameObject obj = GameObject.Find("Manager(Clone)");
		if(!obj)
			return;

		m_ScoreManager = obj.GetComponent<ScoreManager>();
	}
}
