using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Capture))]
public class Player : Character {

	private bool isMovingToTarget;
	private Vector2 moveDir = Vector2.zero;
	private Vector2 targetPosition;

	private Capture captureScript;

	protected override void Start(){
		myFaction = Faction.Good;
		base.Start();
		captureScript = GetComponent<Capture> ();
	}

	void Update(){
		if (!isMovingToTarget) {
			OptionalStartMoving ();
		}
	}

	//start moving if the player presses move buttons and if the targetNode is walkable
	void OptionalStartMoving(Vector2 newPos){
		float instructionX = Input.GetAxis ("Horizontal");
		float instructionY = Input.GetAxis ("Vertical");

		Vector2 currentPos = transform.GetPosition ();
		targetPosition = grid.PositionFromInstruction (currentPos, instructionX, instructionY);

		//if moveinstruction is valid
		if (targetPosition != currentPos) {
			//prevent movement overshoot:
			bool waitOneFrame = false;
			Vector2 newMoveDir = targetPosition - currentPos;
			if (newPos != Vector2.zero && moveDir == newMoveDir) {
				transform.SetPosition (newPos);
				waitOneFrame = true;
			}
			moveDir = newMoveDir;

			//start moving to targetposition
			captureScript.InterruptCapturing();
			StartCoroutine (MoveToTargetPos (targetPosition, waitOneFrame));
		}
	}
	void OptionalStartMoving(){
		OptionalStartMoving (new Vector2 (0, 0));
	}

	IEnumerator MoveToTargetPos(Vector2 targetPos, bool waitAtStart){
		isMovingToTarget = true;
		if (waitAtStart)
			yield return null;

		while(true){
			Vector2 startPos = transform.GetPosition ();
			MoveToPosition (targetPos);

			if(transform.GetPosition () == targetPos){
				Vector2 nextNodeStartPos = startPos + moveDir * (moveSpeed * Time.deltaTime);
				isMovingToTarget = false;
				OptionalStartMoving (nextNodeStartPos);
				yield break;
			}

			yield return null;
		}
	}
}
