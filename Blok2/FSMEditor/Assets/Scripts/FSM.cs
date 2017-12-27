using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FSM {

	public State currentState;
	private AI targetAI;

	public FSM(AI target){
		targetAI = target;
	}

	public void UpdateState(State newState){
		if (currentState != null) {
			currentState.OnExit (targetAI);
		}

		newState.OnEnter (targetAI);
		currentState = newState;
	}

	public void Run(){
		currentState.Run (targetAI);
	}
}