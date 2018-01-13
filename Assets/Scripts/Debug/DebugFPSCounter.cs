using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFPSCounter : MonoBehaviour {

	float m_DeltaTime = 0.0f;
	
	// Update is called once per frame
	void Update () {
		m_DeltaTime += (Time.unscaledDeltaTime - m_DeltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 75;
		style.normal.textColor = new Color (0.0f, 0.5f, 0.0f, 1.0f);
		float msec = m_DeltaTime * 1000.0f;
		float fps = 1.0f / m_DeltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}
