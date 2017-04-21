using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

abstract public class PickupBase : NetworkBehaviour {

	[SerializeField] protected AudioClip m_PickupClip;
	[SerializeField] protected float m_RespawnTime = 10f;
	[SyncVar (hook = "OnEnabledChanged")] protected bool m_Enabled;
	protected AudioSource m_AudioSource;
	protected MeshRenderer m_Mesh;
	protected float m_ElapsedRespawnTime = 0f;

	// Use this for initialization
	protected virtual void Start () {
		m_AudioSource = GetComponent<AudioSource>();
		m_AudioSource.clip = m_PickupClip;
		m_Mesh = GetComponent<MeshRenderer>();
		m_Enabled = true;
	}
	
	// Update is called once per frame
	[ServerCallback]
	protected virtual void Update () {
		if(!m_Enabled){
			m_ElapsedRespawnTime += Time.deltaTime;
			if(m_ElapsedRespawnTime >= m_RespawnTime){
				m_ElapsedRespawnTime = 0f;
				m_Enabled = true;
			}
		}
	}

	protected abstract bool Apply (Collider other);

	[ServerCallback]
	protected virtual void OnTriggerEnter(Collider other) {
        if(other.tag != "Player" || !m_Enabled) return;

		if(Apply(other)){
			m_AudioSource.Play();
			CmdFinishPickup();
		}
    }

	[Command]
	protected virtual void CmdFinishPickup(){
		m_Enabled = false;
	}

	void OnEnabledChanged(bool value){
		m_Enabled = value;
		m_Mesh.enabled = value;
	}
}