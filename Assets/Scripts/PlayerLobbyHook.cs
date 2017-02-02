using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerLobbyHook : LobbyHook {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer){
		LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
		NetworkPlayer gPlayer = gamePlayer.GetComponent<NetworkPlayer>();
		gPlayer.m_PlayerName = lPlayer.playerName;
		gPlayer.m_PlayerColor = lPlayer.playerColor;
	}
}