using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSMData : ScriptableObject {

	[HideInInspector]public List<Node> editorNodeWindows = new List<Node>(3);
	public void AddNodeWindow(Node newEditorNodeWindow){
		Debug.Log (newEditorNodeWindow); 
		Undo.RecordObject (newEditorNodeWindow, unitType + "_FSMData created editorNodeWindow");
		editorNodeWindows.Add (newEditorNodeWindow);
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

	public void SaveData(){//werkt niet! of toch well??!???!
		for (int i = 0; i < editorNodeWindows.Count; i++) {
			EditorUtility.SetDirty (editorNodeWindows [i]);
			Debug.Log ("set node " + i + " to Dirty");
		}

		for (int s = 0; s < states.Count; s++) {
			EditorUtility.SetDirty(states[s]);
		}
		for (int s = 0; s < conditions.Count; s++) {
			EditorUtility.SetDirty(conditions[s]);
		}
	}

	public void RemoveInvalidData(){
		Debug.Log ("conditionCount: " + conditions.Count);
		Debug.Log ("statesCount: " + states.Count);

		for (int i = conditions.Count - 1; i > 0; i--) {
			if (invalidConditions.Contains (i)) {
				invalidConditions.Remove (i);
				conditions.RemoveAt (i);

				Debug.Log ("removing invalid condition " + i); 
			}
			else {
				//Debug.Log ("save condition at " + i);
				EditorUtility.SetDirty (conditions [i]);
			}
		}

		for (int i = states.Count - 1; i > 0; i--) {
			if (invalidStates.Contains (i)) {
				invalidStates.Remove (i);
				states.RemoveAt (i);

				Debug.Log ("removing invalid state " + i); 
			}
			else {
				//Debug.Log ("save state at " + i);
				EditorUtility.SetDirty (states [i]);
			}
		}
	}
}