using UnityEngine;

public delegate void DefaultEvent();//used for death events and such

//this script is being used by Conditions
public class AIEvents : MonoBehaviour {

	public delegate void CollisionDelegate(Collider2D other);
	public event CollisionDelegate onCollisionEnter;
	public event CollisionDelegate onTriggerEnter;

	public event DefaultEvent onDie;
	public event DefaultEvent onTargetDies;

	public event DefaultEvent onStartPatrol;

	void OnCollisionEnter2D(Collision2D coll){
		if (onCollisionEnter != null)
			onCollisionEnter (coll.collider);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (onTriggerEnter != null)
			onTriggerEnter (other);
	}

	public void Die(){
		if (onDie != null) {
			onDie ();
		}
	}

	public void TargetDies(){
		if (onTargetDies != null) {
			onTargetDies ();
		}
	}

	public void StartPatrolling(){
		if (onStartPatrol != null) {
			onStartPatrol ();
		}
	}
}