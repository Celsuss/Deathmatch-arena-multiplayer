﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponPlacement : NetworkBehaviour {

	[SerializeField] Transform m_CameraTransform;
	[SerializeField] Transform m_Hand;
	[SerializeField] Transform m_RightHand;
	[SerializeField] Transform m_LeftHand;
	[SerializeField] Transform m_GunPivot;
	[SerializeField] float m_Treshold = 10f;
	[SerializeField] float m_Smoothing = 5f;
	[SyncVar] float m_Pitch;
	Vector3 m_LastOffset;
	float m_LastSyncedPitch;
	Animator m_Anim;

	// Use this for initialization
	void Start () {
		m_Anim = GetComponent<Animator>();

		if(isLocalPlayer)
			m_GunPivot.parent = m_CameraTransform;
		else
			m_LastOffset = m_Hand.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(isLocalPlayer){
			m_Pitch = m_CameraTransform.localRotation.eulerAngles.x;
			if(Mathf.Abs(m_LastSyncedPitch - m_Pitch) >= m_Treshold){
				CmdUpdatePitch(m_Pitch);
				m_LastSyncedPitch = m_Pitch;
			}
		}
		else{
			Vector3 offset = m_Hand.position - transform.position;
			m_GunPivot.localPosition += offset - m_LastOffset;
			m_LastOffset = offset;
			Quaternion rot = Quaternion.Euler(m_Pitch, 0f, 0f);
			m_GunPivot.localRotation = Quaternion.Lerp(m_GunPivot.localRotation, rot, Time.deltaTime * m_Smoothing);
		}
	}

	[Command]
	void CmdUpdatePitch(float newPitch){
		m_Pitch = newPitch;
	}

	void OnAnimatorIK(){
		m_Anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
		m_Anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
		m_Anim.SetIKPosition(AvatarIKGoal.RightHand, m_RightHand.position);
		m_Anim.SetIKRotation(AvatarIKGoal.RightHand, m_RightHand.rotation);

		m_Anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
		m_Anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
		m_Anim.SetIKPosition(AvatarIKGoal.LeftHand, m_LeftHand.position);
		m_Anim.SetIKRotation(AvatarIKGoal.LeftHand, m_LeftHand.rotation);
	}
}
