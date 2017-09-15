using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//challenge1
public class MovingSphere : MonoBehaviour {
	public float a,b,c,d;

	void Update () {
		float yPos = a + b*Mathf.Sin (Time.time * c + d);
		transform.position = new Vector3 (0,yPos,0);
	}
}
