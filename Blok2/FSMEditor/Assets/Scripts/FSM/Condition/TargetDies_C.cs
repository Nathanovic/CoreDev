using UnityEngine;

[System.Serializable]
public class TargetDies_C : Condition {

	public TargetDies_C(){
		type = ConditionType.requireGameObject;
	}

	public override void Activate () {
		eventScript.onTargetDies += TriggerCondition;
	}
	public override void Deactivate () {
		eventScript.onTargetDies -= TriggerCondition;
	}
}
