using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

	public float mSpeed = 5f;

	void Update () {
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			transform.position = transform.position + (transform.forward * mSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			transform.position = transform.position - (transform.forward * mSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			transform.position = transform.position - (transform.right * mSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			transform.position = transform.position + (transform.right * mSpeed * Time.deltaTime);
		}
	}
}
