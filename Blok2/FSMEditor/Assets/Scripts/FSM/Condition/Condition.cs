using UnityEngine;

//een condition kijkt of er iets gebeurd wat een volgende state kan uitvoeren
//als dit zo is, wordt de volgende state aangeroepen
//condition checking wordt gedaan mbv observer pattern

[System.Serializable]
public abstract class Condition : ScriptableObject {

	private FSM fsm;
	protected AIEvents eventScript;

	[SerializeField]private State nextState;

	[SerializeField]public ConditionType type{ get; protected set; }//used by ConditionNode
	[SerializeField]public bool requireTags{ get; protected set; }//used by ConditionNode
	[SerializeField]protected string[] validTags;

	public void PrepareCondition(State nextState, params string[] tags){//called by ConditionNode.cs
		this.nextState = nextState;
		validTags = tags;
	}

	public void Init(FSM fsm, AIEvents eventScript){//called before an AI starts using his FSM
		this.fsm = fsm;
		this.eventScript = eventScript;
	}

	public abstract void Activate ();
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