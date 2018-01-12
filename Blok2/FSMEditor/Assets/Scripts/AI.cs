using UnityEngine;
using System.Collections.Generic;

//base class for all AI
//the fsm is being ran from void Update, the state-changes are being handled inside the conditions
[RequireComponent(typeof(AIEvents))]
public class AI : MonoBehaviour {

	[SerializeField]private FSMData fsmMaker;
	[SerializeField]private FSM fsm;
	public AIBase stats = new AIBase();

	void Start () {
		fsm = new FSM (this);

		fsm.UpdateState (fsmMaker.states [0]);

		//List<State> states = new List<State>(5);
		//ReadFSMSetup (ref states);
		//fsm.UpdateState (states [0]);
	}

	void ReadFSMSetup(ref List<State> states){
		AIEvents eventScript = GetComponent<AIEvents>();

		//get all states:
		states.Add (new Idle ());
		states.Add (new Attack ());

		//get all conditions:
		Condition collidePlayerCondition = new CollideCondition();
		collidePlayerCondition.PassTags ("Player");
		Condition collideBlockCondition = new CollideCondition();
		collideBlockCondition.PassTags ("Block", "Default");

		//assign conditions to states:
		states[0].InitConditions(collidePlayerCondition);
		states[1].InitConditions(collideBlockCondition);

		//assign condition-state-switches:
		collidePlayerCondition.Init (fsm, eventScript);
		collidePlayerCondition.InitNextState (states [1]);
		collideBlockCondition.Init (fsm, eventScript);
		collideBlockCondition.InitNextState (states [0]);
	}

	void Update(){
		fsm.Run ();
	}

	public void Idle(){
		Debug.Log ("Idle");
	}
	public void Patrol(){
		Debug.Log ("Patrol");
	}
	public void Attack(){
		Debug.Log ("Attack");
	}
}

[System.Serializable]
public class AIBase{
	public int moveSpeed;
}