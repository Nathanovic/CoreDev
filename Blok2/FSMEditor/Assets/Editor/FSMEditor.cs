using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSMEditor : EditorWindow {

	FSMDataHandler dataHandler;
	FSMData selectedFSMData;
	EditorData editorData;

	string fsmDataPath = "Assets/FSMData/";

	[MenuItem("Window/FSMEditor")]
	static void Init(){
		FSMEditor editorWindow = (FSMEditor)EditorWindow.GetWindow<FSMEditor> ();
		editorWindow.Show ();

		Debug.Log ("init");
		editorWindow.dataHandler = new FSMDataHandler ();
		editorWindow.EvaluateSelection ();
	}

	void EvaluateSelection(){
		FSMData currentFSM = dataHandler.GetSelectedDataObject();//pak het geselecteerde scriptableobject where T : FSMData
		if (currentFSM != null && currentFSM != selectedFSMData) {
			selectedFSMData = currentFSM;
			editorData = dataHandler.LoadDataFromScriptableObject (selectedFSMData);
			if (editorData == null) {
				editorData = dataHandler.CreateDefaultData ();
			}
		} else if (selectedFSMData == null && editorData == null) {
			editorData = dataHandler.CreateDefaultData ();			
		}
	}
	
	void OnGUI(){
		GUILayout.BeginHorizontal (GUILayout.MaxWidth (200));
		GUILayout.Label ("Unit type: ", EditorStyles.boldLabel);
		GUILayout.Space (30);
		editorData.unitType = GUILayout.TextField (editorData.unitType, 15, GUILayout.Width(100));
		GUILayout.EndHorizontal ();
		if(GUILayout.Button("Reset nodes", GUILayout.Width(135))){
			editorData = dataHandler.CreateDefaultData ();
		}
		if (selectedFSMData == null) {
			if (GUILayout.Button ("Create new FSM", GUILayout.Width (135))) {
				CreateFsmData ();
			}
		} else {
			if (GUILayout.Button ("Save FSM", GUILayout.Width (135))) {
				dataHandler.SaveDataOnScriptableObject (selectedFSMData, editorData);
			}
		}

		BeginWindows ();//all windows must appear here:
		for (int i = 0; i < editorData.nodeWindows.Count; i++) {
			editorData.nodeWindows [i].DrawWindow (i);
		}
		EndWindows ();
	}

	void OnSelectionChange(){
		EvaluateSelection ();		
	}

	void CreateFsmData(){
		if (editorData.unitType != "") {
			selectedFSMData = ScriptableObject.CreateInstance<FSMData> ();
			selectedFSMData.PassData (editorData);
			string path = fsmDataPath + selectedFSMData.unitType;
			ScriptableObjectUtility.SaveAsset<FSMData> (selectedFSMData, path);
		} else {
			Debug.LogWarning ("Could not create valid FSMData");
		}
	}
}
