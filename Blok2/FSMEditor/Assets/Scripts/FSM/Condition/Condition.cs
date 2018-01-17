using UnityEngine;

//een condition kijkt of er iets gebeurd wat een volgende state kan uitvoeren
//als dit zo is, wordt de volgende state aangeroepen
//condition checking wordt gedaan mbv observer pattern

[System.Serializable]
public abstract class Condition : ScriptableObject {

	private FSM fsm;
	protected AIEvents eventScript;

	private State nextState;

	public bool requireTags = false;
	[SerializeField]protected string[] validTags;
	public ConditionType type;

	public void InitNextState(State nextState){//called by ConditionNode.cs
		this.nextState = nextState;
	}

	public void Init(FSM fsm, AIEvents eventScript){
		this.fsm = fsm;
		this.eventScript = eventScript;
	}

	public void PassTags(params string[] tags){
		validTags = tags;
	}

	public abstract void Activate ();
	public virtual void Activate(GameObject otherObject){//can be used if the condition depends on this otherObject
		Activate ();
	}
	public abstract void Deactivate ();

	protected void TriggerCondition(){
		fsm.UpdateState (nextState);
	}
	protected void TriggerCondition(GameObject otherObject){
		fsm.UpdateState (nextState, otherObject);
	}
}

public enum ConditionType{
	standard,
	passGameObject,
	requireGameObject
}