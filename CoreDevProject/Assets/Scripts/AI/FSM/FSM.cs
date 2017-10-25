using UnityEngine;
using System.Collections.Generic;

//finite state machine for the AI
//different states: 
//-patrolling
//-chasing (= running to the player or to totem)
//-capturing
//-(?)charging (= special move for bosses)
public class FSM {
	 
	public FSM(AI _target){
		target = _target;
	}

	private AI target;
	private Dictionary<StateName, State> myStates = new Dictionary<StateName, State>();
	private State currentState;

	public void AddState(StateName stateKey, State newState){
		myStates.Add (stateKey, newState);
		newState.Init (target);
	}

	public void SetState(StateName stateKey){
		if(currentState != null){
			currentState.Complete ();
			currentState.onState -= SetState;
		}

		currentState = myStates [stateKey];
		currentState.Start ();
		currentState.onState += SetState;
	}

	public void Update(){
		if (currentState != null) {
			currentState.Run ();
		}
	}
}