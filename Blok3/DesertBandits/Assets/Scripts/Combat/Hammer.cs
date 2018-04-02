using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour {

	private Animator anim;
	[SerializeField]private int attackDamage = 1;

	void Start () {
		anim = transform.parent.GetComponent<Animator> ();	
	}
	
	public void Hit(){
		anim.SetTrigger ("hit");
	}

	void OnTriggerEnter(Collider other){
		if (!anim.GetCurrentAnimatorStateInfo (0).IsName ("hit")) {
			return;
		}

		if (other.tag == "Obstacle") {
			Destroy (other.gameObject);
		}
		else if (other.tag == "Enemy") {
			CombatHandler fighter = other.GetComponent<CombatHandler> ();
			if (fighter.ValidTarget ()) {
				fighter.ApplyDamage (attackDamage);
			}
		}
	}
}
