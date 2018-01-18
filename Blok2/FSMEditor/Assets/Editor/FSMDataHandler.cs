using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

public class FSMDataHandler : ScriptableObject {

	string fsmEditorNodesPath = "Assets/FSM_EditorNodes/"; 
	string fsmDataPath = "Assets/FSMData/"; 
	string[] conditionOptions;
	string[] stateOptions;
	private bool createdEntryState;

	public void LoadOptionsFromAssembly(){
		List<string> conditions = new List<string> ();
		List<string> states = new List<string> ();

		Type[] allTypes = Assembly.GetAssembly(typeof(Condition)).GetTypes ();
		foreach (Type type in allTypes) {
			if (type.BaseType == typeof(Condition)) {
				string conditName = type.Name;
				conditName = conditName.Remove (conditName.Length - 2);
				conditions.Add (conditName);
			}
			else if (type.BaseType == typeof(State)) {
				string stateName = type.Name;
				stateName = stateName.Remove (stateName.Length - 2);
				states.Add (stateName);
			}
		}

		conditionOptions = conditions.ToArray ();
		stateOptions = states.ToArray ();
	}

	public FSMData CreateEmptyFSM(){
		FSMData newFSM = ScriptableObject.CreateInstance<FSMData>();
		newFSM.unitType = "NewFSM";
		return newFSM;
	}

	public void ResetFSM(ref FSMData fsmSO){
		createdEntryState = false;
		fsmSO.editorNodeWindows.Clear ();
		fsmSO.invalidConditions.Clear ();
		fsmSO.invalidStates.Clear ();
		fsmSO.conditions.Clear ();
		fsmSO.states.Clear ();
	}

	public void CreateFSMData(ref FSMData fsmSO, bool duplicate){
		if (fsmSO.unitType != "") {
			if (duplicate) {
				Debug.LogWarning ("this feature is not yet implemented");
				//return;
				FSMData newFSMSO = ScriptableObject.CreateInstance <FSMData> ();
				newFSMSO.CopyInit (fsmSO);
				fsmSO = newFSMSO;
			}

			fsmSO.existsInProject = true;
			string path = fsmDataPath + fsmSO.unitType;
			ScriptableObjectUtility.CreateAssetFromScript<FSMData> (fsmSO, path);
		} else {
			Debug.LogWarning ("Could not create valid FSMData because unitType = ''");
		}
	}

	public FSMData GetOtherSelectedDataobject(){
		UnityEngine.Object o = Selection.activeObject;
		if (o == null)
			return null;
		
		Type activeType = o.GetType ();
		if (activeType == typeof(FSMData)) {
			return (FSMData)o;
		}

		return null;
	}

	//wordt aangeroepen als een andere SO wordt geselecteerd of als de editor wordt afgesloten
	public void SaveChangesToFSMData(FSMData fsmSO){
		if(!fsmSO.existsInProject){//als dit SO niet opgeslagen is in het project
			return;
		}

		fsmSO.RemoveInvalidData();
	}

	public void AddNewNode (FSMData fsmSO, string nodeType, Vector2 mousePos){
		Node newNode = null;
		string[] dropdownOptions = null;

		if (nodeType == "Condition") {
			ConditionNode conditNode = ScriptableObject.CreateInstance<ConditionNode> ();
			conditNode.InitConditionNode ();

			dropdownOptions = conditionOptions;
			newNode = conditNode;
		}
		else if (nodeType == "State") {
			StateNode stateNode = ScriptableObject.CreateInstance<StateNode> ();
			stateNode.InitStateNode (!createdEntryState);
			createdEntryState = true;

			dropdownOptions = stateOptions;
			newNode = stateNode;
		}
		else
			Debug.LogWarning ("cant create " + nodeType + " because it is unknown");

		newNode.Init (fsmSO, dropdownOptions, mousePos);
		fsmSO.AddNodeWindow (newNode);
	}

	string EditorNodePathAtIndex(FSMData fsmSO, int i){
		return fsmEditorNodesPath + fsmSO.unitType + "_node_" + i;
	}
}
