using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;

public struct SPlayerScoreInfo{
	public string Name;
	public int Score;
	public int Deaths;
	public SPlayerScoreInfo(string name){
		Name = name;
		Score = 0;
		Deaths = 0;
	}
}

public enum eGameMode{
   eTimed = 0,
   eScored,
   eLast
}

public class SyncListPlayerScoreInfo : SyncListStruct<SPlayerScoreInfo> {}

public class ScoreManager : NetworkBehaviour {

	public eGameMode GameMode{ get{ return m_GameMode; } set{ m_GameMode = value; }}
	public bool GameOver { get{ return m_GameOver; }}

	public SyncListPlayerScoreInfo PlayersInfo{ get{ return m_PlayersInfo; }}
	SyncListPlayerScoreInfo m_PlayersInfo = new SyncListPlayerScoreInfo();
	UIScorePanel m_ScorePanel;
	LobbyManager m_LobbyManager;
	GameObject m_EndGamePanel;
	float m_GameTimer;
	Text m_TimerText;
	bool m_bGameTimer = false;
	bool m_GameOver = false;
	[SyncVar (hook="OnGameModeChanged")] eGameMode m_GameMode;

	void Awake(){
		DontDestroyOnLoad(gameObject);
		LoadScorePanel();

		GameObject obj = GameObject.Find("LobbyManager");
		m_LobbyManager = obj.GetComponent<LobbyManager>();

		m_EndGamePanel = obj.GetComponentInChildren<UIScorePanel>(true).gameObject;
		if(!m_EndGamePanel)
			Debug.Log("Can't find end game panel");

		//m_GameTimer = 120f;
		//m_GameTimer = 10f;
		m_GameTimer = 240f;
	}

	public override void OnStartClient(){
        m_PlayersInfo.Callback = OnSyncListPlayerScoreInfoChanged;
    }

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable(){
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
	// Update is called once per frame
	void Update () {
		if(m_GameMode == eGameMode.eTimed && !m_GameOver){
			if(m_bGameTimer){
				m_GameTimer -= Time.deltaTime;
				UpdateGameTimer();
			}
		}
	}
	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		if(scene.buildIndex == 0){
			Destroy(gameObject);	
		}
		else{
			LoadScorePanel();
			LoadTimerText();
			if(m_GameMode == eGameMode.eTimed){
				m_TimerText.gameObject.SetActive(true);
				m_bGameTimer = true;
			}
		}
    }

	void LoadScorePanel(){
		GameObject obj = GameObject.Find("Score Canvas");
		if(obj)
			m_ScorePanel = obj.GetComponent<UIScorePanel>();
	}

	void LoadTimerText(){
		GameObject obj = GameObject.Find("Game Timer Text");
		if(obj){
			m_TimerText = obj.GetComponent<Text>();
			m_TimerText.gameObject.SetActive(false);
		}
	}

	void UpdateGameTimer(){
		float m = Mathf.Floor(m_GameTimer/60);
		int s = (int)(m_GameTimer - (m*60));

		if(m <= 0 && s <= 0){
			m = 0;
			s = 0;
			ShowEndGameScreen();	
		}

		m_TimerText.text = m + ":" + s;
	}

	void ShowEndGameScreen(){
		m_GameOver = true;
		m_EndGamePanel.SetActive(true);	
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	[Command]
	public void CmdAddPlayer(string name){
		SPlayerScoreInfo playerInfo = new SPlayerScoreInfo(name);
		m_PlayersInfo.Add(playerInfo);
	}

	[Command]
	public void CmdRemovePlayer(string name){
		for(int i = 0; i < m_PlayersInfo.Count; ++i){
			if(m_PlayersInfo[i].Name == name){
				m_PlayersInfo.RemoveAt(i);
				break;
			}
		}
	}

	[Command]
	public void CmdIncrementScore(string name){
		for(int i = 0; i < m_PlayersInfo.Count; ++i){
			if(m_PlayersInfo[i].Name == name){
				SPlayerScoreInfo info = m_PlayersInfo[i];
				info.Score = info.Score + 1;
				m_PlayersInfo[i] = info;
				break;
			}
			if(i == m_PlayersInfo.Count-1)
				Debug.LogWarning("Can't find player " + name);
		}
	}

	[Command]
	public void CmdIncrementDeaths(string name){
		for(int i = 0; i < m_PlayersInfo.Count; ++i){
			if(m_PlayersInfo[i].Name == name){
				SPlayerScoreInfo info = m_PlayersInfo[i];
				info.Deaths = info.Deaths + 1;
				m_PlayersInfo[i] = info;
				break;
			}
			if(i == m_PlayersInfo.Count-1)
				Debug.LogWarning("Can't find player " + name);
		}
	}

	void OnSyncListPlayerScoreInfoChanged(SyncListPlayerScoreInfo.Operation operation, int index){
		m_ScorePanel.UpdateScoreTexts(m_PlayersInfo);
	}

	void OnGameModeChanged(eGameMode value){
		m_GameMode = (eGameMode)value;
		m_LobbyManager.gameModeInfo.text = m_GameMode.ToString().Substring(1);
	}
}