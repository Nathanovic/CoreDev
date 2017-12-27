using UnityEngine;
using UnityEditor;

public class StateNode : Node {

	private string stateName;

	public StateNode(){
		windowRect = new Rect (50, 50, 300, 50);
		windowTitle = "state node";
		windowType = "state";
	}

	protected override void DoMyWindow (int windowID) {
		GUILayout.Label ("Some state");

		base.DoMyWindow (windowID);
	}

	public State GetStateFromNode (){
		Debug.Log ("it works!");
		State s = new Attack();
		return s;
	}
}