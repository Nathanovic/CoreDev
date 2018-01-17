using UnityEngine;

[System.Serializable]
public class TargetDies_C : Condition {

	public override void Activate () {
		eventScript.onTargetDies += TriggerCondition;
	}
	public override void Deactivate () {
		eventScript.onTargetDies -= TriggerCondition;
	}
}
