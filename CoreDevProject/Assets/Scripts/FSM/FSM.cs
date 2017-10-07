using UnityEngine;

public class FSM {
	 
	public FSM(State startState, Character _target){
		SetState (startState);
		target = _target;
	}

	private State currentState;
	private Character target;

	public void SetState(State newState){
		if(currentState != null){
			currentState.Complete (target);
			currentState.onState -= SetState;
		}

		newState.Start (target);
		newState.onState += SetState;

		currentState = newState;
	}

	public void Update(){
		if (currentState != null) {
			currentState.Run (target);
		}
	}
}