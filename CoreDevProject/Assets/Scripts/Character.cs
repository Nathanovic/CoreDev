using UnityEngine;
using System.Collections;

public delegate void CharacterEvent(Character sender);

//abstract class for ai & player
public abstract class Character : MonoBehaviour {

	private float checkFrontRayLength = 0.2f;
	public float moveSpeed = 1f;

	private FSM fsm;

	public CharacterEvent onDeath;

	//abstract methods
	public abstract void ControlMe ();
	public virtual void EndMe (){
		onDeath (this);
	}

	//have to do with FSM methods:
	protected virtual void Start(){
		fsm = new FSM (new InControl(), this);
		checkFrontRayLength += GetComponent<CircleCollider2D> ().radius;
	}
	//controls the entire behaviour of the character except for OnEnabled and void Start
	public void UpdateFSM(){
		fsm.Update ();
	}
		
	//other methods:
	protected void MoveMe (Vector2 moveInstruction){
		Vector2 currentPos = transform.GetPosition ();
		Vector2 translation = (moveInstruction * moveSpeed * Time.deltaTime);
		transform.SetPosition (currentPos + translation);
	}

	protected Collider2D GetFrontCollider (Vector2 lookDir){
		RaycastHit2D hit = Physics2D.Raycast (transform.GetPosition(), lookDir, checkFrontRayLength);

		if (hit != null) {
			return hit.collider;
		} 
		else {
			return null;
		}
	}
}
