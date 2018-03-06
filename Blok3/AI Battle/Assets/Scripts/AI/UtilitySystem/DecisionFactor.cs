using UnityEngine;

namespace AI_UtilitySystem{

	//returns a normalized value based on a Stat
	[CreateAssetMenu(fileName = "Decision Factor", menuName = "Utility System/Decision Factor", order = 1)]
	public class DecisionFactor : ScriptableObject{

		public Stat stat;
		public FactorDecider factorSolver;

		public float minStatValue;
		public float maxStatValue;
		public AnimationCurve statCurve;

		public float Value(AIStats statsModel){
			float statValue = statsModel.GetStatValue (stat);

			switch (factorSolver) {
			case FactorDecider.Greater:
				return (statValue > minStatValue) ? 1f : 0f;
			case FactorDecider.Less:
				return (statValue < maxStatValue) ? 1f : 0f;
			case FactorDecider.Curve:
				float t = NormalizedStat (statValue);
				return statCurve.Evaluate(t);
			case FactorDecider.Random:
				return Random.Range (minStatValue, maxStatValue);
			default:
				Debug.LogWarning ("Unkown factor solver: " + factorSolver.ToString ());
				return 0f;
			}
		}

		float NormalizedStat(float statValue){
			statValue = Mathf.Clamp (statValue, minStatValue, maxStatValue);
			statValue -= minStatValue;
			float statDiff = maxStatValue - minStatValue;
			return statValue / statDiff;
		}
	}

	public enum FactorDecider{
		Greater, Less, Curve, Random
	}
}
