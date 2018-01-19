using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

public class SyncListPlayerScoreInfo : SyncListStruct<SPlayerScoreInfo> {}

public class ScoreManager : NetworkBehaviour {

	SyncListPlayerScoreInfo m_PlayersInfo = new SyncListPlayerScoreInfo();

	public SyncListPlayerScoreInfo PlayersInfo{ get{ return m_PlayersInfo; }}

	UIScorePanel m_ScorePanel;

	void Awake(){
		GameObject scorePanel = GameObject.Find("Score Canvas");
		if(scorePanel)
			m_ScorePanel = scorePanel.GetComponent<UIScorePanel>();
		else
			Debug.LogWarning("Can't find score panel");
	}

	public override void OnStartClient(){
        m_PlayersInfo.Callback = OnSyncListPlayerScoreInfoChanged;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
