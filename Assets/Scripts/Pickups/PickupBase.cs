using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

abstract public class PickupBase : NetworkBehaviour {

	[SerializeField] protected AudioClip m_PickupClip;
	[SerializeField] protected float m_RespawnTime = 10f;
	[SyncVar (hook = "OnEnabledChanged")] protected bool m_Enabled = true;
	protected AudioSource m_AudioSource;
	protected MeshRenderer m_Mesh;
	protected float m_ElapsedRespawnTime = 0f;

	// Use this for initialization
	protected virtual void Start () {
		m_AudioSource = GetComponent<AudioSource>();
		if(m_PickupClip != null)
				m_AudioSource.clip = m_PickupClip;
		m_Mesh = GetComponent<MeshRenderer>();
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

	[Command]
	protected abstract void CmdApply (GameObject other);

	[ServerCallback]
	protected virtual void OnTriggerEnter(Collider other) {
        if(other.tag != "Player" || !m_Enabled) return;

		CmdApply(other.gameObject);
    }

	[Command]
	protected virtual void CmdFinishPickup(){
		// TODO: Play audio in RPC
		if(m_AudioSource != null)
			m_AudioSource.Play();
		m_Enabled = false;
	}

	void OnEnabledChanged(bool value){
		m_Enabled = value;
		if(m_Mesh != null)
			m_Mesh.enabled = value;
	}
}