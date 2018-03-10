using UnityEngine;

namespace AI_UtilitySystem{

	//this state makes the agent run away from some source of danger
	//for example this could be used after damage is applied to this agent to show that the agent is afraid
	[CreateAssetMenu(fileName = "Run Away", menuName = "Utility System/State: Run away", order = 1)]
	public class RunAway : State{

		public float minRunDist = 2f;
		public float maxRunDist = 7f;

		public float moveSpeed = 3f;

		private float runAwayDist;
		private float passedDistance;

		public override void EnterState () {
			passedDistance = 0f;
			runAwayDist = UnityEngine.Random.Range (minRunDist, maxRunDist);
			base.EnterState ();

			controller.TryFacingDanger (true);
		}

		public override void Run () {
			if (passedDistance > runAwayDist || statsModel.ObjectAhead()) {
				EndState ();
				return;
			}

			//run away:
			passedDistance += moveSpeed * Time.deltaTime;
			controller.MoveForward (moveSpeed);
		}

		protected override void EndState () {
			base.EndState ();
			controller.TryFacingDanger ();
		}

		public override State GetCopy () {
			RunAway copyState = ScriptableObject.CreateInstance<RunAway> ();
			SetCopyBase <RunAway>(ref copyState, "Run Away");

			copyState.minRunDist = minRunDist;
			copyState.maxRunDist = maxRunDist;
			copyState.moveSpeed = moveSpeed;

			return copyState;
		}
	}
}
