using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour {
	private Rigidbody2D rb;
	public float speed = 1f;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		rb.AddForce (transform.right * Input.GetAxis ("Horizontal") * speed);
	}
}
