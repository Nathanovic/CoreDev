using UnityEngine;

namespace AI_UtilitySystem{
	[CreateAssetMenu(fileName = "Die", menuName = "Utility System/State: Die", order = 1)]
	public class Die : State{

		public float fadeOutTime = 2f;
		private float rendererAlpha = 1f;

		public override void Run () {
			rendererAlpha -= Time.deltaTime / fadeOutTime;
			controller.FadeSelfOut (rendererAlpha);

			if (rendererAlpha <= 0f) {
				EndState ();
			}
		}

		protected override void EndState () {
			controller.DestroySelf ();
			base.EndState ();
		}

		public override State GetCopy () {
			Die copyState = ScriptableObject.CreateInstance<Die> ();
			SetCopyBase <Die>(ref copyState, "Die");

			copyState.fadeOutTime = fadeOutTime;

			return copyState;
		}
	}
}

