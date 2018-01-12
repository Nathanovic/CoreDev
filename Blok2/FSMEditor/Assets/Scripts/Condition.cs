using UnityEngine;

//een condition kijkt of er iets gebeurd wat een volgende state kan uitvoeren
//als dit zo is, wordt de volgende state aangeroepen
//condition checking wordt gedaan mbv observer pattern

public abstract class Condition : ScriptableObject {

	private FSM fsm;
	protected AIEvents eventScript;

	private State nextState;

	public bool requireTags = false;
	protected string[] validTags;

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
	public abstract void Deactivate ();

	protected void TriggerCondition(){
		fsm.UpdateState (nextState);
	}
}

[System.Serializable]
public class CollideCondition : Condition{

	public CollideCondition(){
		requireTags = true;
	}

	public override void Activate(){
		Debug.Log ("activate collide condition");
		eventScript.onCollisionEnter += EvaluateCollision;
	}
	public override void Deactivate (){
		Debug.Log ("deactivate condition");
		eventScript.onCollisionEnter -= EvaluateCollision;
	}

	void EvaluateCollision(Collider2D other){
		foreach (string validTag in validTags) {
			if (other.tag == validTag) {
				Debug.Log ("we are colliding with: " + validTag + "!");
				TriggerCondition ();
			}
		}
	}
}

[System.Serializable]
public class DieCondition : Condition{

	public override void Activate(){
		eventScript.onDie += TriggerCondition;
	}
	public override void Deactivate(){
		eventScript.onDie -= TriggerCondition;
	}
}