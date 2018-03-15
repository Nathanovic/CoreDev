using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Patrol", menuName = "Utility System/State: Patrol", order = 1)]
	public class Patrol : State{

		private float passedMoveDist;
		private float targetMoveDist;

		public float moveSpeed = 1.5f;
		public float minMoveDist = 0.2f;
		public float maxMoveDist = 1f;

		public override void EnterState () {
			base.EnterState ();

			passedMoveDist = 0f;
			targetMoveDist = UnityEngine.Random.Range (minMoveDist, maxMoveDist);

			//random facing rotation:
			if (Random.Range (0f, 1f) > 0.5f) {
				controller.FlipFacingDirection ();
			}

			//make sure we'r not trying to move through a wall
			if (statsModel.ObstacleAhead()) {
				controller.FlipFacingDirection ();
			}
		}

		public override void Run () {
			//move forward:
			controller.MoveForward(moveSpeed);
			passedMoveDist += moveSpeed * Time.deltaTime;

			//stop patrolling if the agent moved the targetMoveDist or if there is a obstacle in front of him
			if (passedMoveDist >= targetMoveDist || statsModel.ObstacleAhead ()) {
				EndState ();
			}
		}

		public override State GetCopy () {
			Patrol copyState = ScriptableObject.CreateInstance<Patrol> ();
			SetCopyBase <Patrol>(ref copyState, "Patrol");

			copyState.moveSpeed = moveSpeed;
			copyState.minMoveDist = minMoveDist;
			copyState.maxMoveDist = maxMoveDist;

			return copyState;
		}
	}
}
