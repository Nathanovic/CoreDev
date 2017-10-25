using UnityEngine;
using System.Collections;

public class Capture : MonoBehaviour {

	//base values:
	private Character mainScript;
	private Faction myFaction{
		get{ 
			return mainScript.myFaction;
		}
	}

	//totem values:
	private Totem targetTotem;
	private CaptureState myState = CaptureState.none;
	[SerializeField]private float captureTime = 5f;//lager is sneller, dus beter
	public event NoParamEvent onStartCapturing;//subscribed to by Ghost.cs
	public event NoParamEvent onCapturedTotem;//subscribed to by CaptureState

	private void Start () {
		mainScript = GetComponent<Character> ();
		mainScript.onDeath += InterruptCapturing;
		onStartCapturing += StartCapturing;
		onCapturedTotem += StopCapturing;
	}
	
	private void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Totem" && myState == CaptureState.none){
			Totem totem = other.GetComponent<Totem> ();
			if(totem.CanCaputureMe(myFaction)){
				mainScript.onReachedTargetNode += ReachedTotem;
				targetTotem = totem;
			}
		}
	}

	private void ReachedTotem(){
		mainScript.onReachedTargetNode -= ReachedTotem;
		if (targetTotem.CanCaputureMe (myFaction)) {
			onStartCapturing ();
		}
	}
	private void StartCapturing(){
		myState = CaptureState.capturing;
		StartCoroutine(targetTotem.CaptureMe (captureTime, onCapturedTotem));
	}
	private void StopCapturing(){
		myState = CaptureState.none;
		targetTotem.StopCapturing ();
		targetTotem = null;
	}

	public void InterruptCapturing(){//tried when the player moves, called by CaptureState when the state is ended
		if(myState == CaptureState.capturing){
			StopCapturing ();
		}
	}
	private void InterruptCapturing(Character sender){//called onDeath
		InterruptCapturing ();
	}

	private enum CaptureState{
		none,
		capturing
	}
}
