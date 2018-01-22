using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class UIScorePanel : MonoBehaviour {

	[SerializeField] GameObject m_ScorePanel;
	[SerializeField] Text m_NamesText;
	[SerializeField] Text m_ScoreText;
	[SerializeField] Text m_DeathsText;
	ScoreManager m_ScoreManager;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(!m_ScorePanel)
			return;

		if(Input.GetButtonDown("Toggle Score Panel")){
			if(!m_ScoreManager){
				FindScoreManager();
				UpdateScoreTexts(m_ScoreManager.PlayersInfo);
			}

			m_ScorePanel.SetActive(!m_ScorePanel.activeSelf);
		}
	}

	public void UpdateScoreTexts(SyncListPlayerScoreInfo playerInfo){
		if(!m_NamesText ||!m_ScoreText || !m_DeathsText)
			return;

		m_NamesText.text = "";
		m_ScoreText.text = "";
		m_DeathsText.text = "";

		foreach(SPlayerScoreInfo info in playerInfo){
			m_NamesText.text = m_NamesText.text + info.Name + "\n";
			m_ScoreText.text = m_ScoreText.text + info.Score.ToString() + "\n";
			m_DeathsText.text = m_DeathsText.text + info.Deaths.ToString() + "\n";
		}
	}

	void FindScoreManager(){
		GameObject obj = GameObject.Find("Manager(Clone)");
		if(!obj)
			return;

		m_ScoreManager = obj.GetComponent<ScoreManager>();
	}
}