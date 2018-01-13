using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScorePanel : MonoBehaviour {

	struct PlayerScoreInfo{

	}

	[SerializeField] GameObject m_ScorePanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Toggle Score Panel")){
			m_ScorePanel.SetActive(!m_ScorePanel.activeSelf);
		}
	}
}