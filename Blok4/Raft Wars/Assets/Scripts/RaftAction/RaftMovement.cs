using UnityEngine;
using UnityEngine.Networking;
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
				if (isServer) {
					FinishAction ();
				}
			}
		}
	}

	public override void EvaluateInput (out bool succes) {
		Vector3 moveOffset = Vector3.zero;
		int moveAnim = 0;
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			moveOffset = -Vector3.right * moveDist;
			moveAnim = 3;
		}
		else if (Input.GetKeyUp (KeyCode.RightArrow)) {
			moveOffset = Vector3.right * moveDist;
			moveAnim = 1;
		}
		else if (Input.GetKeyUp (KeyCode.DownArrow)) {
			moveOffset = -Vector3.up * moveDist;
			moveAnim = 2;
		}
		else if (Input.GetKeyUp (KeyCode.UpArrow)) {
			moveOffset = Vector3.up * moveDist;
			moveAnim = 0;
		}

		if (moveOffset != Vector3.zero) {
			succes = true;
			if (!isServer) {
				CmdMoveRaft (moveOffset, moveAnim);
			}
			else {
				CmdMoveRaft (moveOffset, moveAnim);
			}
		} 
		else {
			succes = false;
		}
	}

	[Command]
	private void CmdMoveRaft(Vector3 movement, int moveAnim){
		RpcMoveRaft (movement, moveAnim);
	}

	[ClientRpc]
	private void RpcMoveRaft(Vector3 movement, int moveAnim){
		isMoving = true;
		targetPos = transform.position + movement;
		anim.SetInteger ("direction", moveAnim);
	}
}
