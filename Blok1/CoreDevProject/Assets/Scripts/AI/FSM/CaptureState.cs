using UnityEngine;
using System.Collections;

public class CaptureState : State{

	private Capture captureScript;

	public override void Init (AI _target){
		base.Init (_target);
		captureScript = _target.GetComponent<Capture> ();
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
