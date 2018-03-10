using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Charge Forward", menuName = "Utility System/State: Charge fwd", order = 1)]
	public class Charge : State{

		[Header("Charge forward:")]
		public int chargeDamage = 1;
		public string chargeAnimName = "chargeForward";

		public float maxChargeDist = 10f;
		public float chargeSpeed = 5f;

		private float passedChargeDist;

		public override void EnterState () {
			passedChargeDist = 0f;
			statsModel.onTriggerOther += TriggerOther;
			if (statsModel.ObjectAhead ()) {
				statsModel.forward *= -1;
			}

			base.EnterState ();
			controller.SetAnimBool (chargeAnimName, true);
			controller.TryFacingDanger ();
		}

		public override void Run () {
			if (statsModel.ObjectAhead ()) {
				EndState ();
				return;
			}

			//charge forward:
			controller.MoveForward(chargeSpeed);
			passedChargeDist += chargeSpeed * Time.deltaTime;
		}

		private void TriggerOther(Collider2D other){
			IAttackable target = other.GetComponent<IAttackable> ();
			if (target != null && target != (IAttackable)controller) {
				target.ApplyDamage (chargeDamage);
				if (target == statsModel.target) {
					EndState ();
				}
			}
		}

		protected override void EndState () {
			statsModel.onTriggerOther -= TriggerOther;
			controller.SetAnimBool (chargeAnimName, false);
			base.EndState ();
		}

		public override State GetCopy () {
			Charge copyState = ScriptableObject.CreateInstance<Charge> ();
			SetCopyBase <Charge>(ref copyState, "Charge Forward");

			copyState.chargeAnimName = chargeAnimName;
			copyState.chargeDamage = chargeDamage;
			copyState.maxChargeDist = maxChargeDist;
			copyState.chargeSpeed = chargeSpeed;

			return copyState;
		}
	}
}
