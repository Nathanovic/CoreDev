using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Attack", menuName = "Utility System/State: Attack", order = 1)]
	public class StandardAttack : State{

		private float pastAttackTime;
		public float waitForDamageTime = 0.6f;
		public int attackDamage = 1;
		public string attackAnimName = "stab";

		public override void EnterState () {
			base.EnterState ();
			pastAttackTime = 0f;
			controller.SetTrigger(attackAnimName);
			controller.TryFacingDanger ();
		}

		public override void Run () {
			pastAttackTime += Time.deltaTime;

			if (pastAttackTime >= waitForDamageTime) {
				//make sure that we only apply damage to our target if it is still relevant:
				CalculateUtility ();
				if (utilityValue > 0.1f){
					TryDamageTarget ();
				}

				EndState ();
			}
		}

		private void TryDamageTarget(){
			if (statsModel.target != null) {
				statsModel.target.ApplyDamage (attackDamage);
			}
		}

		public override State GetCopy () {
			StandardAttack copyState = ScriptableObject.CreateInstance<StandardAttack> ();
			SetCopyBase <StandardAttack>(ref copyState, "Standard Attack");

			copyState.waitForDamageTime = waitForDamageTime;
			copyState.attackAnimName = attackAnimName;
			copyState.attackDamage = attackDamage;

			return copyState;
		}
	}
}

