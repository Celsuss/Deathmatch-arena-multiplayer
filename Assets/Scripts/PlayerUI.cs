using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	public static PlayerUI Instance;
	[SerializeField] Image m_Crosshair;
	[SerializeField] Text m_GameStatusText;
	[SerializeField] Text m_HealthText;
	[SerializeField] Text m_AmmoText;
	[SerializeField] Text m_KillsText;
	[SerializeField] AudioSource m_DeathSound;

	void Awake(){
		if(!Instance)
			Instance = this;
		else if(Instance != this)
			Destroy(gameObject);
	}

	void Reset(){
		m_Crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
		m_GameStatusText = GameObject.Find("Game Status Text").GetComponent<Text>();
		m_HealthText = GameObject.Find("Health Text").GetComponent<Text>();
		m_AmmoText = GameObject.Find("Ammo Text").GetComponent<Text>();
		m_KillsText = GameObject.Find("Kills Text").GetComponent<Text>();
		m_DeathSound = GameObject.Find("Death Sound").GetComponent<AudioSource>();
	}

	public void Initialize(){
		m_Crosshair.enabled = true;
		m_GameStatusText.text = "";
	}

	public void HideCrosshair(){
		m_Crosshair.enabled = false;
	}

	public void PlayDeathAudio(){
		if(!m_DeathSound.isPlaying)
			m_DeathSound.Play();
	}

	public void SetKills(int amount){
		m_KillsText.text = "Kills: " + amount.ToString();
	}

	public void SetAmmo(int magAmount, int ammoAmount){
		m_AmmoText.text = "Ammo: " + magAmount.ToString() + "/" + ammoAmount.ToString();
	}

	public void SetHealth(int amount){
		m_HealthText.text = "Health: " + amount.ToString();
	}

	public void WriteGameStatusText(string text){
		m_GameStatusText.text = text;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
