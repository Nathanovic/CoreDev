using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeEditor : EditorWindow {

	public GUISkin mySkin;

	private List<Node> nodeWindows = new List<Node> ();
	private Vector2 mousePos;
	private Node selectedNode;
	bool makeTransitionMode = false;

	private delegate void NodeDeleted(Node node);
	private event NodeDeleted onNodeDeleted;

	[MenuItem("Window/Node Editor")]
	static void ShowEditor(){
		NodeEditor editor = EditorWindow.GetWindow<NodeEditor> ();
	}

	void OnGUI(){
		//nodeWindows.Clear ();//QQQ
		//GUI.skin = mySkin;
		//GUI.Window (0, new Rect (20, 20, 200, 140), QQQDrawRect, "titlee");
		Event e = Event.current;
		mousePos = e.mousePosition;

		if (nodeWindows.Count > 0) {
			GUILayout.Label ("title1: " + nodeWindows [0].windowTitle);
		}

		if (e.button == 1 && !makeTransitionMode) {
			if (e.type == EventType.mouseDown) {
				int selectIndex = -1;
				bool clickedOnWindow = MouseOverWindow (out selectIndex);

				GenericMenu menu = new GenericMenu ();

				if (!clickedOnWindow) {
					menu.AddItem (new GUIContent ("Add Input Node"), false, ContextCallback, "inputNode");
					menu.AddItem (new GUIContent ("Add Output Node"), false, ContextCallback, "outputNode");
					menu.AddItem (new GUIContent ("Add Calculation Node"), false, ContextCallback, "calcNode");
					menu.AddItem (new GUIContent ("Add Comparison Node"), false, ContextCallback, "compNode");
				}
				else {
					menu.AddItem (new GUIContent ("Make Transition"), false, ContextCallback, "makeTransition");
					menu.AddSeparator ("");
					menu.AddItem (new GUIContent ("Delete Node"), false, ContextCallback, "deleteNode");
				}

				menu.ShowAsContext ();
				e.Use ();
			}
		}
		else if (e.button == 0 && e.type == EventType.mouseDown && makeTransitionMode) {
			int selectIndex = -1;
			bool clickedOnWindow = MouseOverWindow (out selectIndex);

			//will the selected node be used as input:
			if (clickedOnWindow && !nodeWindows [selectIndex].Equals (selectedNode)) {
				nodeWindows [selectIndex].SetInput ((BaseInputNode)selectedNode, mousePos);
				makeTransitionMode = false;
				selectedNode = null;
			}
			else if (!clickedOnWindow) {
				makeTransitionMode = false;
				selectedNode = null;
			}

			e.Use ();
		}
		else if (e.button == 0 && e.type == EventType.mouseDown && !makeTransitionMode) {
			int selectIndex = -1;
			bool clickedOnWindow = MouseOverWindow (out selectIndex);

			if (clickedOnWindow) {
				BaseInputNode nodeToChange = nodeWindows [selectIndex].ClickedOnInput (mousePos);
				if (nodeToChange) {
					selectedNode = nodeToChange;
					makeTransitionMode = true;
				}
			}
		}

		//draw nodeCurve from selectedNode to mousePos
		if (makeTransitionMode && selectedNode != null) {
			Rect mouseRect = new Rect (e.mousePosition.x, e.mousePosition.y, 10, 10);
			DrawNodeCurve (selectedNode.windowRect, mouseRect);
			Repaint ();
		}

		foreach (Node n in nodeWindows) {
			n.DrawCurves ();
		}

		BeginWindows ();//all windows must appear here:

		for (int i = 0; i < nodeWindows.Count; i++) {
			nodeWindows [i].windowRect = GUILayout.Window (i, nodeWindows [i].windowRect, 
				DrawNodeWindow, nodeWindows [i].windowTitle);
		}

		EndWindows ();
	}

	Rect QQQRect = new Rect (50, 50, 200, 200);
	void QQQDrawRect(int windowID){
		if(GUILayout.Button("Explode")){
			Debug.Log ("button pressed");		
		}
		GUI.DragWindow ();
	}

	void ContextCallback(object obj){
		Node newNode = null;
		int nodeHeight = 170;

		string clb = obj.ToString ();
		switch (clb) {
		case "inputNode":
			InputNode inputNode = ScriptableObject.CreateInstance<InputNode>();
			inputNode.windowRect = new Rect (mousePos.x, mousePos.y, 200, nodeHeight);
			onNodeDeleted += inputNode.NodeDeleted;
			nodeWindows.Add (inputNode);
			break;
		case "outputNode":
			newNode = new OutputNode () as Node;
			nodeHeight = 100;
			break;
		case "calcNode":
			newNode = new CalcNode ();
			break;
		case "compNode":
			newNode = new ComparisonNode () as Node;
			break;
		default:
			int selectIndex = -1;
			bool clickedOnWindow = MouseOverWindow (out selectIndex);
			if (clickedOnWindow) {
				Node clickedNode = nodeWindows [selectIndex];
				if (clb.Equals ("makeTransition")) {
					selectedNode = clickedNode;
					makeTransitionMode = true;
				}
				else if (clb.Equals ("deleteNode")) {
					onNodeDeleted (clickedNode);
					onNodeDeleted -= clickedNode.NodeDeleted;
					nodeWindows.RemoveAt (selectIndex);
				}
				else {
					Debug.LogError ("Unknown callback: " + clb);//QQQ
				}
			}
			break;
		}

		if(newNode){
			newNode.windowRect = new Rect (mousePos.x, mousePos.y, 200, nodeHeight);
			onNodeDeleted += newNode.NodeDeleted;
			nodeWindows.Add (newNode);
		}
	}
		
	bool MouseOverWindow(out int selectIndex){
		selectIndex = -1;
		int nodeCount = nodeWindows.Count;
		for (int i = 0; i < nodeCount; i++) {
			if (nodeWindows [i].windowRect.Contains (mousePos)) {
				selectIndex = i;
				return true;
			}
		}

		return false;
	}

	void DrawNodeWindow(int windowID){
		nodeWindows [windowID].DrawWindow ();
		GUI.DragWindow ();
	}

	public static void DrawNodeCurve(Rect start, Rect end){
		Vector3 startPos = new Vector3 (start.x + start.width / 2, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3 (end.x + end.width / 2, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Color shadowCol = new Color (0, 0, 0, .06f);

		for (int i = 0; i < 3; i++) {//draw shadow:
			Handles.DrawBezier (startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
		}
		//draw the actual curve:
		Handles.DrawBezier (startPos, endPos, startTan, endTan, Color.black, null, 1);
	}
}
