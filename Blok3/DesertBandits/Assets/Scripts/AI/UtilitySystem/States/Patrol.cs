using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Patrol", menuName = "Utility System/State: Patrol", order = 1)]
	public class Patrol : State{

		public float stopPatrolDist = 1.2f;
		public float moveSpeed = 1.5f;

		public override void EnterState () {
			base.EnterState ();

			Vector3 targetPosition = new Vector3 ();

			controller.SetAgentSpeed (moveSpeed);
			controller.UpdateDestination (targetPosition);
		}

		public override void Run () {
			//stop patrolling if the agent moved the targetMoveDist or if there is a obstacle in front of him
			if (statsModel.DistToDestination() <= stopPatrolDist) {
				EndState ();
			}
		}

		public override State GetCopy () {
			Patrol copyState = ScriptableObject.CreateInstance<Patrol> ();
			SetCopyBase <Patrol>(ref copyState, "Patrol");

			copyState.stopPatrolDist = stopPatrolDist;
			copyState.moveSpeed = moveSpeed;

			return copyState;
		}
	}
}
