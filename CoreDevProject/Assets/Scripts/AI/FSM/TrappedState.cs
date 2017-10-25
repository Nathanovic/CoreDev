using UnityEngine;
using System.Collections;

public class TrappedState : State {

	public override void Init (AI _target){
		base.Init (_target);
		captureScript.onCapturedTotem += EndCapturing;
	}

	private void EndCapturing(){
		onState (StateName.patrolling);		
	}

	//state implementation:
	public override void Run(){
		if(baseAI.chaseState.CanChaseTarget()){
			onState (StateName.chasing);
		}
	}
	public override void Complete(){
		captureScript.InterruptCapturing ();
	}
}
