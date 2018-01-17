using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSMEditor : EditorWindow {

	private FSMDataHandler dataHandler;
	private FSMData currentFSMData;

	private List<Node> nodeWindows{
		get{
			return currentFSMData.editorNodeWindows;
		}
	}
	private string unitType{
		get{
			return currentFSMData.unitType;
		}
		set{ 
			currentFSMData.unitType = value;
		}
	}

	private bool fsmExistsInProject{
		get{ 
			return currentFSMData.existsInProject;
		}
	}

	private Node transitionStartNode;
	private Vector2 inputMousePos;

	private Vector2 relationArrowSize = new Vector2 (10, 15);

	[MenuItem("Window/FSMEditor")]
	static void Init(){
		FSMEditor editorWindow = (FSMEditor)EditorWindow.GetWindow<FSMEditor> ();
		editorWindow.Show ();

		editorWindow.dataHandler = ScriptableObject.CreateInstance<FSMDataHandler> ();
		editorWindow.dataHandler.LoadOptionsFromAssembly ();
		editorWindow.EvaluateSelection ();
	}
	 
	void EvaluateSelection(){
		FSMData otherFSM = dataHandler.GetOtherSelectedDataobject();//pak het geselecteerde scriptableobject where T : FSMData
		if (otherFSM != null && otherFSM != currentFSMData) {
			if(currentFSMData != null){//first save changes, is eigenlijk niet nodig omdat als er iets veranderd, dit automatisch in de SO wordt opgeslagen
				dataHandler.SaveChangesToFSMData (currentFSMData);
			}

			currentFSMData = otherFSM;
		}
		else if (currentFSMData == null) {
			Debug.Log ("Could not load data, creating default data");
			currentFSMData = dataHandler.CreateEmptyFSM ();
		}
		else {
			return;
		}
	}
	
	void OnGUI(){
		DrawMainSection ();
		DrawGUIButtons ();
		EvaluateInput ();
		DrawNodeWindows ();
	}

	void OnDisable(){
		dataHandler.SaveChangesToFSMData (currentFSMData);
	}

	void DrawMainSection(){
		if (!fsmExistsInProject) {
			GUILayout.BeginHorizontal (GUILayout.MaxWidth (200));
			GUILayout.Label ("Unit type: ", EditorStyles.boldLabel);
			GUILayout.Space (30);
			unitType = GUILayout.TextField (unitType, 15, GUILayout.Width (100));
			GUILayout.EndHorizontal ();		
		}
		else {
			GUILayout.Label (unitType, EditorStyles.boldLabel);
		}
	}

	void DrawGUIButtons(){
		if (!fsmExistsInProject) {
			if (GUILayout.Button ("Create", GUILayout.Width (135))) {
				dataHandler.CreateFSMData (ref currentFSMData, fsmExistsInProject);
			}
		} 
		else {
			if (GUILayout.Button ("Reset FSM", GUILayout.Width (135))) {
				dataHandler.ResetFSM (ref currentFSMData);
			}
		}
	}

	void DrawNodeWindows(){
		BeginWindows ();//all windows must appear here:
		for (int i = 0; i < nodeWindows.Count; i++) {
			nodeWindows [i].DrawWindow (i);
			nodeWindows [i].DrawRelationArrows (DrawRelationArrow);
		}
		EndWindows ();		
	}

	void EvaluateInput(){
		if (!fsmExistsInProject) {
			return;
		}

		Event e = Event.current;
		if (transitionStartNode == null) {
			if (e.button == 1 && e.type == EventType.mouseDown) {
				inputMousePos = e.mousePosition;
				int selectIndex = MouseOverWindow ();

				GenericMenu menu = new GenericMenu ();

				if (selectIndex == -1) {
					menu.AddItem (new GUIContent ("Add Condition Node"), false, OnNodeSelected, "Condition");
					menu.AddItem (new GUIContent ("Add State Node"), false, OnNodeSelected, "State");
				}
				else {
					menu.AddItem (new GUIContent ("Make Transition"), false, EnableTransitionMode, selectIndex);
					menu.AddSeparator ("");
					menu.AddItem (new GUIContent ("Delete Node"), false, RemoveNode, selectIndex);
				}

				menu.ShowAsContext ();
			}
		}
		else {
			inputMousePos = e.mousePosition;
			DrawRelationArrow (inputMousePos, transitionStartNode.nodeConnectionPoint (inputMousePos));
			if (e.button == 0 && e.type == EventType.mouseUp) {
				int selectIndex = MouseOverWindow ();
				MakeTransition (selectIndex);
			}
		}
	}

	int MouseOverWindow(){
		for (int i = 0; i < nodeWindows.Count; i++) {
			if (nodeWindows [i].windowRect.Contains (inputMousePos)) {
				return i;
			}
		}

		return -1;
	}

	void OnNodeSelected (object nodeType){
		dataHandler.AddNewNode (currentFSMData, nodeType.ToString(), inputMousePos);
	}

	void EnableTransitionMode(object fromNodeIndex){
		transitionStartNode = nodeWindows [(int)fromNodeIndex];
	}
	void MakeTransition(int nodeIndex){
		if (nodeIndex != -1) {
			transitionStartNode.ConnectToNode (nodeWindows [nodeIndex]);
			transitionStartNode = null;
		}
	}

	void RemoveNode(object nodeIndex){
		int nIndex = (int)nodeIndex;
		nodeWindows [nIndex].DestroyNode();
		nodeWindows.RemoveAt (nIndex);
	}

	void OnSelectionChange(){
		EvaluateSelection ();		
	}

	void DrawRelationArrow(Vector3 startPos, Vector3 endPos){
		Handles.color = Color.black;
		Handles.DrawLine (startPos, endPos);

		float avgX = (startPos.x + endPos.x) / 2f;
		float avgY = (startPos.y + endPos.y) / 2f;
		Vector3 avgPos = new Vector3 (avgX, avgY, 0);
		Vector3 arrowDir = (endPos - startPos).normalized;
		Vector3 perpendicDirection = Vector3.Cross (arrowDir, Vector3.forward).normalized;

		Vector3 arrowPoint = avgPos + arrowDir * relationArrowSize.y;
		Vector3 arrowTailL = avgPos + perpendicDirection * relationArrowSize.x;
		Vector3 arrowTailR = avgPos + perpendicDirection * -relationArrowSize.x;
		Handles.DrawLine (arrowTailL, arrowPoint);
		Handles.DrawLine (arrowTailR, arrowPoint);
	}
}
