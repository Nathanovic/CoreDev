using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the player can trigger this block to fall
public class EvilBlock : MonoBehaviour {

	private Rigidbody2D rb;//rb type is standard kinematic
	private Animator anim;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Space)) {
			Invoke ("Fall", 0.5f);
			anim.SetBool ("fall", true);
		}
	}

	void Fall(){
		rb.bodyType = RigidbodyType2D.Dynamic;		
		rb.gravityScale = 6f;
	}
}
