using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSMData : ScriptableObject {

	[HideInInspector]public bool existsInProject = false;
	public List<Node> editorNodeWindows = new List<Node>(3);
	public void AddNodeWindow(Node newEditorNodeWindow){
		editorNodeWindows.Add (newEditorNodeWindow);
		AssetDatabase.AddObjectToAsset (newEditorNodeWindow, this);
	}

	public string unitType;

	public List<Condition> conditions = new List<Condition>();
	public List<int> invalidConditions = new List<int> ();//conditions die geremoved zijn
	public List<State> states = new List<State> ();
	public List<int> invalidStates = new List<int> ();//states die geremoved zijn

	public void CopyInit(FSMData otherFSM){
		editorNodeWindows = otherFSM.editorNodeWindows;
		unitType = otherFSM.unitType;
		conditions = otherFSM.conditions;
		states = otherFSM.states;
	}

	public void LinkStatesAndConditions(out bool succes){
		succes = true;
		for (int i = 0; i < editorNodeWindows.Count; i++) {
			bool nodeLinkedSuccesfully = true;
			editorNodeWindows [i].PrepareModel (out nodeLinkedSuccesfully);
			if (!nodeLinkedSuccesfully) {
				succes = false;
			}
		}
	}

	//remove the nodes that the user deleted:
	public void RemoveInvalidData(){
		for (int i = conditions.Count - 1; i > 0; i--) {
			if (invalidConditions.Contains (i)) {
				invalidConditions.Remove (i);
				conditions.RemoveAt (i);

				Debug.Log ("removing invalid condition " + i); 
			}
		}

		for (int i = states.Count - 1; i > 0; i--) {
			if (invalidStates.Contains (i)) {
				invalidStates.Remove (i);
				states.RemoveAt (i);

				Debug.Log ("removing invalid state " + i); 
			}
		}
	}
}