using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientView : MonoBehaviour {

	float mSpeed = 90f;

	void Update () {
		Vector3 _orient = transform.localRotation.eulerAngles;
		_orient.y += Input.GetAxis("Mouse X") * mSpeed * Time.deltaTime;
		transform.localRotation = Quaternion.Euler(_orient);
	}
}
