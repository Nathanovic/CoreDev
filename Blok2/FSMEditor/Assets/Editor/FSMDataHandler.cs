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
				Debug.LogWarning ("this feature is not yet implemented");
				return;
				FSMData newFSMSO = ScriptableObject.CreateInstance <FSMData> ();
				newFSMSO.CopyInit (fsmSO);
				fsmSO = newFSMSO;
			}

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

	public void LoadFSMData(FSMData fsmSO){
		int fileIndex = 0;
		while (true) {
			string path = EditorNodePathAtIndex (fsmSO, fileIndex);
			if (File.Exists (path)) {
				Debug.Log ("loading Node at path: " + path);
				Node newNode = AssetDatabase.LoadAssetAtPath<Node> (path);
				fsmSO.editorNodeWindows.Add (newNode);

				fileIndex++;
			} else {
				break;
			}
		}
	}

	//wordt aangeroepen als een andere SO wordt geselecteerd of als de editor wordt afgesloten
	public void SaveChangesToFSMData(FSMData fsmSO){
		//Undo.RecordObject (fsmSO, "Saved FSMData: " + fsmSO.unitType);
		if(false){//als dit SO niet opgeslagen is
			return;
		}


		for (int i = 0; i < fsmSO.editorNodeWindows.Count; i++) {
			if (!fsmSO.editorNodeWindows [i].existsInProject) {
				fsmSO.editorNodeWindows [i].existsInProject = true;
				string path = EditorNodePathAtIndex (fsmSO, i);
				ScriptableObjectUtility.CreateAssetFromScript<Node> (fsmSO.editorNodeWindows [i], path);
			}
		}

		//fsmSO.RemoveInvalidData();
		//fsmSO.SaveData();
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

	string EditorNodePathAtIndex(FSMData fsmSO, int i){
		return fsmEditorNodesPath + fsmSO.unitType + "_node_" + i;
	}
}
//Type eventScriptType = eventScript.GetType ();
//EventInfo[] eventInfo = eventScriptType.GetEvents (BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
//int eventCount = eventInfo.Length;
//string[] options = new string[eventCount];
//for(int i = 0; i < eventCount; i ++){
//	options [i] = eventInfo [i].Name;
//}
