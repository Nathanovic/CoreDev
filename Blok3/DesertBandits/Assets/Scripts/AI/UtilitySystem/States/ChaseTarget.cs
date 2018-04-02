using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Chase Target", menuName = "Utility System/State: Chase Target", order = 1)]
	public class ChaseTarget : State{

		[Header("Charge forward:")]
		public float stopChargeDist = 1.2f;
		public float chargeSpeed = 5f;

		public override void EnterState () {
			base.EnterState ();
			controller.SetAnimBool ("idle", true);
			controller.SetAgentSpeed (chargeSpeed);
			controller.UpdateTargetDestination();
		}

		public override void Run () {
			//check if we reached our destination, and stop if so
			if (statsModel.SimpleDistToTarget() <= stopChargeDist || statsModel.nmAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete) {
				EndState ();
				return;
			}

			//charge forward:
			controller.UpdateTargetDestination();
		}

		protected override void EndState () {
			base.EndState ();
		}

		public override State GetCopy () {
			ChaseTarget copyState = ScriptableObject.CreateInstance<ChaseTarget> ();
			SetCopyBase <ChaseTarget>(ref copyState, "Chase Target");

			copyState.stopChargeDist = stopChargeDist;
			copyState.chargeSpeed = chargeSpeed;

			return copyState;
		}
	}
}
