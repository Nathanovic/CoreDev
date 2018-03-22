using UnityEngine;

namespace AI_UtilitySystem{

	//this state makes the agent run away from some source of danger
	//for example this could be used after damage is applied to this agent to show that the agent is afraid
	[CreateAssetMenu(fileName = "Evade", menuName = "Utility System/State: Evade", order = 1)]
	public class Evade : State{

		public float minJumpDist = 1f;
		public float maxJumpDist = 2f;

		public float jumpSpeed = 5.5f;

		public float minEvadeDist = 2f;
		public float maxEvadeDist = 3f;
		public float evadeSpeed = 3f;

		private bool jumping;
		private float jumpDist;
		private float evadeDist;
		private float passedDistance;

		public override void EnterState () {
			jumping = true;
			passedDistance = 0f;
			jumpDist = UnityEngine.Random.Range (minJumpDist, maxJumpDist);
			evadeDist = UnityEngine.Random.Range (minEvadeDist, maxEvadeDist);
			base.EnterState ();

			controller.SetForward (Vector2.up);
		}

		public override void Run () {
			if ((!jumping && passedDistance > evadeDist) || statsModel.ObstacleAhead ()) {
				EndState ();
				return;
			} else if (jumping && passedDistance > jumpDist) {
				StartEvade ();
			}

			//run away:
			float moveSpeed = jumping ? jumpSpeed : evadeSpeed;
			passedDistance += moveSpeed * Time.deltaTime;
			if (jumping)
				controller.MoveUp (moveSpeed);
			else
				controller.MoveForward (moveSpeed);
		}

		void StartEvade(){
			jumping = false;
			controller.TryFacingDanger ();
		}

		protected override void EndState () {
			base.EndState ();
			controller.TryFacingDanger ();
		}

		public override State GetCopy () {
			Evade copyState = ScriptableObject.CreateInstance<Evade> ();
			SetCopyBase <Evade>(ref copyState, "Evade");

			copyState.minJumpDist = minJumpDist;
			copyState.maxJumpDist = maxJumpDist;
			copyState.jumpSpeed = jumpSpeed;
			copyState.minEvadeDist = minEvadeDist;
			copyState.maxEvadeDist = maxEvadeDist;
			copyState.evadeSpeed = evadeSpeed;

			return copyState;
		}
	}
}
