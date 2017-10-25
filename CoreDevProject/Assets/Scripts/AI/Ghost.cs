using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Capture))]
public class Ghost : AI {

	protected override void Start(){
		base.Start ();

		fsm.AddState (StateName.capturing, new CaptureState());
		Capture captureScript = GetComponent<Capture> ();
		captureScript.onStartCapturing += StartCapturing;
	}

	public void StartCapturing(){
		fsm.SetState (StateName.capturing);
	}
}
