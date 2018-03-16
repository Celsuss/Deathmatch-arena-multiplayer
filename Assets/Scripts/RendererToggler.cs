using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererToggler : MonoBehaviour {

	[SerializeField] float m_TurnOnDelay = .1f;
    [SerializeField] float m_TurnOffDelay = .1f;   	// Time for death animation
    [SerializeField] bool m_EnabledOnLoad = false;

    Renderer[] m_Renderers;

    void Awake () 
    {
        m_Renderers = GetComponentsInChildren<Renderer> (true); 

        if (m_EnabledOnLoad)
            EnableRenderers ();
        else
            DisableRenderers ();
    }

    //Method used by our Unity events to show and hide the player
    public void ToggleRenderersDelayed(bool isOn)
    {
        if (isOn)
            Invoke ("EnableRenderers", m_TurnOnDelay);
        else
            Invoke ("DisableRenderers", m_TurnOffDelay);
    }

    public void EnableRenderers()
    {
        for (int i = 0; i < m_Renderers.Length; i++) 
        {
            m_Renderers [i].enabled = true;
        }
    }

    public void DisableRenderers()
    {
        for (int i = 0; i < m_Renderers.Length; i++) 
        {
            m_Renderers [i].enabled = false;
        }
    }

    //Will be used to change the color of the players for different options
    public void ChangeColor(Color newColor)
    {
        for (int i = 0; i < m_Renderers.Length; i++) 
        {
            m_Renderers [i].material.color = newColor;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
