using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSMData : ScriptableObject {

	[HideInInspector]public List<Node> editorNodeWindows = new List<Node>(3);
	public void AddNodeWindow(Node newEditorNodeWindow){
		Debug.Log (newEditorNodeWindow); 
		//Undo.RegisterCompleteObjectUndo (newEditorNodeWindow, unitType + "_FSMData_node");
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

	public void ClearScriptableObjects(){
		editorNodeWindows.Clear ();

//		for (int i = 0; i < editorNodeWindows.Count; i++) {
//			EditorUtility.SetDirty (editorNodeWindows [i]);
//		}
//
//		for (int i = 0; i < states.Count; i++) {
//			EditorUtility.SetDirty(states[i]);
//		}
//		for (int i = 0; i < conditions.Count; i++) {
//			//EditorUtility.SetDirty(conditions[i]);
//		}
//
//		Undo.FlushUndoRecordObjects ();
	}

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