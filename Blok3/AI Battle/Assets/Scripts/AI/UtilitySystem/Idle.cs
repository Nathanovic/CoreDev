using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Idle", menuName = "Utility System/State: Idle", order = 1)]
	public class Idle : State{

		[Header("Idle for a random period between:")]
		public float minIdleDuration;
		public float maxIdleDuration;
		private float idleDuration;
		private float passedIdleTime;

		public override void EnterState () {
			idleDuration = UnityEngine.Random.Range (minIdleDuration, maxIdleDuration);
			base.EnterState ();

			controller.TryFacingDanger ();
		}

		public override void Run () {
			passedIdleTime += Time.deltaTime;
			if (passedIdleTime > idleDuration) {
				EndState ();
				return;
			}
		}

		public override State GetCopy () {
			Idle copyState = ScriptableObject.CreateInstance<Idle> ();
			SetCopyBase <Idle>(ref copyState, "Idle");

			copyState.minIdleDuration = minIdleDuration;
			copyState.maxIdleDuration = maxIdleDuration;

			return copyState;
		}
	}
}
