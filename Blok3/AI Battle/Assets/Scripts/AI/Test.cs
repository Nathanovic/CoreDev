using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_UtilitySystem;

public class Test : MonoBehaviour {

	private AIStats myStats;
	public DecisionFactor factor;

	void Start(){
		myStats = GetComponent<AIStats> ();
	}

	void Update(){
		if (Input.GetKeyUp (KeyCode.Return)) {
			Debug.Log ("Factor: " + factor.Value (myStats));
		}
	}
}
