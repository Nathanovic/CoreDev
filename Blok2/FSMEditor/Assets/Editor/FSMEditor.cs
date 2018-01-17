using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSMEditor : EditorWindow {

	//public GUISkin customGUISkin;
	public Texture arrow;
	FSMDataHandler dataHandler;
	FSMData currentFSMData;

	List<Node> nodeWindows{
		get{
			return currentFSMData.editorNodeWindows;
		}
	}
	string unitType{
		get{
			return currentFSMData.unitType;
		}
		set{ 
			currentFSMData.unitType = value;
		}
	}

	bool fsmExistsInProject{
		get{ 
			return currentFSMData.existsInProject;
		}
	}

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

			dataHandler.LoadFSMData (otherFSM);
			currentFSMData = otherFSM;
		}
		else if (currentFSMData == null) {
			Debug.Log ("Could not load data, creating default data");
			currentFSMData = dataHandler.CreateDefaultFSM ();
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
		if(GUILayout.Button("Reset nodes", GUILayout.Width(135))){
			currentFSMData = dataHandler.CreateDefaultFSM ();
		}

		string createFSMText = fsmExistsInProject ? "Duplicate" : "Create";
		if (GUILayout.Button (createFSMText, GUILayout.Width (135))) {
			dataHandler.CreateFSMData (ref currentFSMData, fsmExistsInProject);
		}
	}

	void DrawNodeWindows(){
		BeginWindows ();//all windows must appear here:
		for (int i = 0; i < nodeWindows.Count; i++) {
			nodeWindows [i].DrawWindow (i);
			nodeWindows [i].DrawRelationArrows (DrawArrow);
		}
		EndWindows ();		
	}

	private Node transitionStartNode;
	private Vector2 inputMousePos;
	void EvaluateInput(){
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
			DrawArrow (inputMousePos, transitionStartNode.nodeConnectionPoint (inputMousePos));
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

	void DrawArrow(Vector3 startPos, Vector3 endPos){
		Handles.color = Color.white;
		Handles.DrawLine (startPos, endPos);

		float avgX = (startPos.x + endPos.x) / 2f;
		float avgY = (startPos.y + endPos.y) / 2f;
		Vector2 avgPos = new Vector2 (avgX, avgY);
		Vector2 arrowSize = new Vector2 (50, 50);

		Vector2 dirToEnd = (endPos - startPos).normalized;
		Vector2 arrowPos = (Vector2)startPos + dirToEnd;
		Quaternion arrowRot = Quaternion.LookRotation (endPos - startPos);

		Handles.ArrowHandleCap(//ugly
			0,
			arrowPos,
			arrowRot,
			100f,
			EventType.Repaint
		);

		//Rect arrowRect = new Rect (avgPos, arrowSize);
		//GUI.DrawTexture (arrowRect, arrow);
	}
}
