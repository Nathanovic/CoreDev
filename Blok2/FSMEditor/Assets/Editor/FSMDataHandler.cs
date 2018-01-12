using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

public class FSMDataHandler : ScriptableObject {

	string fsmDataPath = "Assets/FSMData/"; 
	string[] conditionOptions;
	string[] stateOptions;

	public void LoadOptionsFromAssembly(){
		List<string> conditions = new List<string> ();
		List<string> states = new List<string> ();

		Type[] allTypes = Assembly.GetAssembly(typeof(Condition)).GetTypes ();
		foreach (Type type in allTypes) {
			if (type.BaseType == typeof(Condition)) {
				conditions.Add (type.Name);
			}
			else if (type.BaseType == typeof(State)) {
				states.Add (type.Name);
			}
		}

		conditionOptions = conditions.ToArray ();
		stateOptions = states.ToArray ();
	}

	public FSMData CreateDefaultFSM(){
		Debug.Log ("create default data");
		FSMData newFSM = ScriptableObject.CreateInstance<FSMData>();

		AddNewNode (newFSM, "State", new Vector2(150,150));
		AddNewNode (newFSM, "Condition", new Vector2(350,150));
		//conditionNode.ConnectToState (stateNode);

		newFSM.unitType = "NewFSM";
		return newFSM;
	}

	public void CreateFSMData(ref FSMData fsmSO, bool duplicate){
		if (fsmSO.unitType != "") {
			if (duplicate) {
				Debug.LogWarning ("Duplicate is not yet implemented");
				return;
				FSMData newFSMSO = ScriptableObject.CreateInstance <FSMData> ();
				newFSMSO.CopyInit (fsmSO);
				fsmSO = newFSMSO;
			}

			Debug.Log ("Try creating fsm: " + fsmSO.unitType);
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

	public void SaveChangesToFSMData(FSMData fsmSO){
		//Undo.RecordObject (fsmSO, "Saved FSMData: " + fsmSO.unitType);
		fsmSO.RemoveInvalidData();
		fsmSO.SaveData();
	}

	public void AddNewNode (FSMData fsmSO, string nodeType, Vector2 mousePos){
		Node newNode = null;
		string[] dropdownOptions = null;

		if (nodeType == "Condition") {
			newNode = ScriptableObject.CreateInstance<ConditionNode> ();
			dropdownOptions = conditionOptions;
		}
		else if (nodeType == "State") {
			newNode = ScriptableObject.CreateInstance<StateNode> ();
			dropdownOptions = stateOptions;
		}
		else
			Debug.LogWarning ("cant create " + nodeType + " because it is unknown");

		newNode.Init (fsmSO, dropdownOptions, mousePos);
		fsmSO.AddNodeWindow (newNode);
	}
}
//Type eventScriptType = eventScript.GetType ();
//EventInfo[] eventInfo = eventScriptType.GetEvents (BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
//int eventCount = eventInfo.Length;
//string[] options = new string[eventCount];
//for(int i = 0; i < eventCount; i ++){
//	options [i] = eventInfo [i].Name;
//}
