using UnityEngine;
using System.Collections;

public class TrappedState : State {

	public override void Init (AI _target){
		base.Init (_target);
		_target.onTrapActivated += EndTrappedState;
	}

	private void EndTrappedState(){
		onState (StateName.patrolling);		
	}

	//state implementation:
	public override void Complete(){
		baseAI.RemoveState (StateName.trapped);
	}
}
