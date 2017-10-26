using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Capture))]
public class Player : Character {

	private bool isMovingToTarget;
	private Vector2 moveDir = Vector2.zero;
	private Vector2 targetPosition;

	private Capture captureScript;

	[SerializeField]private GoodTrap trap;
	private bool carryingTrap = false;

	protected override void Start(){
		myFaction = Faction.Good;
		base.Start();
		captureScript = GetComponent<Capture> ();

		onDeath += GameManager.Instance.GameOver;
	}

	private void Update(){
		if (!isMovingToTarget) {
			OptionalStartMoving ();
		}

		if(Input.GetKeyUp(KeyCode.Space)){
			if (carryingTrap) {
				carryingTrap = false;
				trap.PlaceMe (transform.GetPosition ());
			}
			else if (trap.CanTriggerMe) {
				carryingTrap = true;
				trap.Dismantle ();
			}
		}
	}

	//start moving if the player presses move buttons and if the targetNode is walkable
	private void OptionalStartMoving(Vector2 newPos){
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

	private void OptionalStartMoving(){
		OptionalStartMoving (new Vector2 (0, 0));
	}

	private IEnumerator MoveToTargetPos(Vector2 targetPos, bool waitAtStart){
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
