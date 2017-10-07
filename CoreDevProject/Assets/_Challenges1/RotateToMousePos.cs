using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMousePos : MonoBehaviour {
	public float smoothRotationTime = 0.01f;

	public float rotationSpeed = 2f;

	void Update () {
		Vector3 visibleMousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f);
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (visibleMousePos);
		//transform.LookAt (mousePos);

//		Vector3 dirToMousePos = mousePos - transform.position;
//		float distToMousePos = Mathf.Sqrt (dirToMousePos.x * dirToMousePos.x + dirToMousePos.y * dirToMousePos.y);
//
//		float asRatio = dirToMousePos.x / distToMousePos;
//		float desiredAngle = Mathf.Acos (asRatio) * Mathf.Rad2Deg;
//		if (dirToMousePos.y > 0f)
//			desiredAngle = 180f - desiredAngle + 180f;
//
//		//smoothing rotation doesnt work:
//		//float cVel = 0f;
//		//float slerpedAngle = Mathf.SmoothDampAngle (transform.eulerAngles.x, desiredAngle, ref cVel, smoothRotationTime);
//
//		transform.eulerAngles = new Vector3 (desiredAngle,90f,-90f);

		Quaternion newLookDir = Quaternion.LookRotation (mousePos - transform.position, -Vector3.forward);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, newLookDir, rotationSpeed);
	}
}
