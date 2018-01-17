
[System.Serializable]
public class Die_C : Condition{

	public override void Activate(){
		eventScript.onDie += TriggerCondition;
	}
	public override void Deactivate(){
		eventScript.onDie -= TriggerCondition;
	}
}