using UnityEngine;
using UnityEngine.Events;

public class RaftMovement : RaftActionPerformer {

	[SerializeField]private float moveDist = 1f;
	[SerializeField]private float moveSpeed;

	private bool isMoving;
	private Vector3 targetPos;

	private void Update(){
		if (isMoving) {
			float step = moveSpeed * Time.deltaTime;
			Vector3 newPos = Vector3.MoveTowards (transform.position, targetPos, step);
			transform.position = newPos;

			if (newPos == targetPos) {
				isMoving = false;
				FinishAction ();
			}
		}
	}

	public override void EvaluateInput (out bool succes) {
		Vector3 moveOffset = Vector3.zero;
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			moveOffset = -Vector3.right * moveDist;
		}
		else if (Input.GetKeyUp (KeyCode.RightArrow)) {
			moveOffset = Vector3.right * moveDist;
		}
		else if (Input.GetKeyUp (KeyCode.DownArrow)) {
			moveOffset = -Vector3.up * moveDist;
		}
		else if (Input.GetKeyUp (KeyCode.UpArrow)) {
			moveOffset = Vector3.up * moveDist;
		}

		if (moveOffset != Vector3.zero) {
			succes = true;
			MoveRaft (moveOffset);
		} 
		else {
			succes = false;
		}
	}

	private void MoveRaft(Vector3 movement){
		isMoving = true;
		targetPos = transform.position + movement;
	}
}
