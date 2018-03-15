using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Charge Forward", menuName = "Utility System/State: Charge fwd", order = 1)]
	public class Charge : State{

		[Header("Charge forward:")]
		public int chargeDamage = 1;
		public string chargeAnimName = "chargeForward";

		public float maxChargeDist = 10f;
		public float chargeSpeed = 5f;

		public LayerMask targetLM;

		private float passedChargeDist;

		public override void EnterState () {
			base.EnterState ();
			passedChargeDist = 0f;
			controller.SetAnimBool (chargeAnimName, true);
			controller.TryFacingDanger ();
		}

		public override void Run () {
			//check for obstacles:
			if (statsModel.ObstacleAhead ()) {
				EndState ();
				return;
			}

			//check if we can hit a target:
			Debug.DrawLine(controller.Position(), statsModel.forward * statsModel.weaponRange, Color.red);
			IAttackable attackable = GetAttackableTarget ();
			TryDamageTarget (attackable);

			//charge forward:
			controller.MoveForward(chargeSpeed);
			passedChargeDist += chargeSpeed * Time.deltaTime;
		}

		//get the target that is in attack range
		private IAttackable GetAttackableTarget(){
			RaycastHit2D hit = Physics2D.Raycast (controller.Position (), statsModel.forward, statsModel.weaponRange, targetLM);
			if (hit.collider != null) {
				IAttackable target = hit.collider.GetComponent<IAttackable> ();
				return target;
			}

			return null;
		}

		//apply damage to target (if we can!)
		private void TryDamageTarget(IAttackable target){
			if (target != null) {
				target.ApplyDamage (chargeDamage);
				EndState ();
			}
		}

		protected override void EndState () {
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
			copyState.targetLM = targetLM;

			return copyState;
		}
	}
}
