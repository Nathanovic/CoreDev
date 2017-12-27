using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

public class FSMDataHandler {

	public void SaveDataOnScriptableObject(FSMData dataHolder, EditorData fsmData){
		dataHolder.PassData (fsmData);
	}

	public EditorData LoadDataFromScriptableObject(FSMData data){
		if (data != null) {
			return data.editorData;
		} else {
			return null;
		}
	}

	public EditorData CreateDefaultData(){
		Debug.Log ("create default data");
		string[] stateOptions = ConditionOptions ();

		StateNode stateNode = ScriptableObject.CreateInstance<StateNode>();
		ConditionNode conditionNode = ScriptableObject.CreateInstance<ConditionNode> ();
		conditionNode.Init (stateOptions);

		string aiName = "FSM_AI";
		EditorData myData = new EditorData (aiName, stateNode, conditionNode);
		Debug.Log ("name: " + myData.unitType);
		return myData;
	}

	public FSMData GetSelectedDataObject(){
		UnityEngine.Object o = Selection.activeObject;
		if (o == null)
			return null;
		
		Type activeType = o.GetType ();
		if (activeType == typeof(FSMData)) {
			return (FSMData)o;
		}

		return null;
	}

	string[] ConditionOptions(){
		List<string> conditions = new List<string> ();

		Type[] allTypes = Assembly.GetExecutingAssembly ().GetTypes ();
		foreach (Type type in allTypes) {
			if (type.BaseType == typeof(Condition)) {
				Debug.Log ("condition:  " + type.Name);
				conditions.Add (type.Name);
			}
		}

		return conditions.ToArray();
	}
}

[System.Serializable]
public class EditorData{

	public EditorData(){}
	public EditorData(string _unitType, params Node[] nodes){
		unitType = _unitType;
		nodeWindows = new List<Node> (nodes.Length);
		for (int i = 0; i < nodes.Length; i++) {
			nodeWindows.Add (nodes[i]);
		}
	}

	public List<Node> nodeWindows;
	public string unitType;
}

//Type eventScriptType = eventScript.GetType ();
//EventInfo[] eventInfo = eventScriptType.GetEvents (BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
//int eventCount = eventInfo.Length;
//string[] options = new string[eventCount];
//for(int i = 0; i < eventCount; i ++){
//	options [i] = eventInfo [i].Name;
//}
