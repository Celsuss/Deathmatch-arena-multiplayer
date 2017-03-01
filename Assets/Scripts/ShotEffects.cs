using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEffects : MonoBehaviour {

	[SerializeField] ParticleSystem m_MuzzleFlash;
	[SerializeField] AudioSource m_GunAudio;
	[SerializeField] GameObject m_ImpactPrefab;

	ParticleSystem m_ImpactEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Initialize(){
		//m_ImpactEffect = Instantiate(m_ImpactPrefab).GetComponent<ParticleSystem>();
	}

	public void PlayShotEffects(){
		m_MuzzleFlash.Stop(true);
		m_MuzzleFlash.Play(true);
		//m_GunAudio.Stop();
		//m_GunAudio.Play();
	}

	public void PlayImpactEffect(Vector3 impactPos){
		//m_ImpactEffect.transform.position = impactPos;
		//m_ImpactEffect.Stop();
		//m_ImpactEffect.Play();
	}
}
