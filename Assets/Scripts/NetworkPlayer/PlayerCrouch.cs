using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerCrouch : NetworkBehaviour {
	CharacterController m_Controller;
	CapsuleCollider m_Capsule;
	Camera m_Camera;
	Transform m_CameraTransform;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	bool m_Crouch;
	[SyncVar (hook = "OnCrouchingChanged")] bool m_Crouching;

	// Use this for initialization
	void Start () {
		m_Controller = GetComponent<CharacterController>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;
		m_Crouch = false;

		if(isLocalPlayer){
			m_Camera = Camera.main;
			m_CameraTransform = m_Camera.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer) return;

		if(!m_Crouch)
             m_Crouch = CrossPlatformInputManager.GetButtonDown("Crouch");

		if (m_Crouch && !m_Crouching){
			CmdCrouch(true);
			m_Crouch = false;
		}
		else if(m_Crouch && m_Crouching){
			CmdCrouch(false);
			m_Crouch = false;
		}
	}

	[Command]
	void CmdCrouch(bool crouch){
		m_Crouching = crouch;
	}

	void OnCrouchingChanged(bool value){
		m_Crouching = value;
		if(value){
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Controller.height = m_Controller.height / 2f;
			m_Controller.center = m_Controller.center / 2f;

			if(isLocalPlayer)
				m_CameraTransform.transform.position = new Vector3(m_CameraTransform.transform.position.x, m_CameraTransform.transform.position.y - m_Capsule.height, m_CameraTransform.transform.position.z);
		}
		else{
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Controller.height = m_CapsuleHeight;
			m_Controller.center = m_CapsuleCenter;

			if(isLocalPlayer)
				m_CameraTransform.transform.position = new Vector3(m_CameraTransform.transform.position.x, m_CameraTransform.transform.position.y + (m_Capsule.height / 2f), m_CameraTransform.transform.position.z);
		}
	}

	public bool IsCrouching(){
		return m_Crouching;
	}
}