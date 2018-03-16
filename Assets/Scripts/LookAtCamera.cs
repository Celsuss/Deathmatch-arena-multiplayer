using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LookAtCamera : MonoBehaviour {

	Transform m_Camera;

	// Use this for initialization
	void Start () {
		m_Camera = Camera.main.transform;
        Assert.IsNotNull(m_Camera);
    }

	void LateUpdate(){
		if(!m_Camera) return;

		transform.rotation = Quaternion.LookRotation(transform.position - m_Camera.position);
	}
}
