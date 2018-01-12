using UnityEngine;
using UnityEditor;
using System;

public abstract class Node : ScriptableObject {
	//scriptableObject is needed to have functions like OnDestroy; so I dont need it!

	public bool existsInProject = false;

	protected FSMData linkedFSM;
	protected int linkedModelIndex = -1;

	public Rect windowRect;
	protected string windowTitle;
	protected string windowType;
	protected const int MAX_STRING_LENGTH = 15;

	protected string dropdownDescription;
	protected string[] dropdownOptions;//de verschillende mogelijke voorwaarden 
	private int previousDropdownIndex;
	protected int selectedDropdownIndex;//de index van de geselecteerde voorwaarde/state

	public delegate void OnNodeDestroyed(Node thisNode);
	public event OnNodeDestroyed onNodeDestroyed;

	public void Init(FSMData fsmSO, string[] options, Vector2 startPos){//zowel stateNodes als conditionNodes hebben een dropdown die hier gebruik van maakt
		linkedFSM = fsmSO;
		dropdownOptions = options;
		selectedDropdownIndex = 0;
		windowRect.position = startPos;

		CreateModelFromIndex ();
	}

	protected void DoMyDropdown(){
		selectedDropdownIndex = EditorGUILayout.Popup (dropdownDescription, selectedDropdownIndex, dropdownOptions);
		EvaluateDropdownIndex ();
	}
	protected void EvaluateDropdownIndex(){//van ofwel de condition, ofwel de state die geselecteerd is
		if (selectedDropdownIndex != previousDropdownIndex) {
			previousDropdownIndex = selectedDropdownIndex;
			CreateModelFromIndex ();
		}
	}
	protected abstract void CreateModelFromIndex ();

	public void DrawWindow(int windowID){
		windowRect = GUILayout.Window (windowID, windowRect, DoMyWindow, windowTitle, GUILayout.MinWidth(150));
	}

	protected virtual void DoMyWindow(int windowID){
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Box (windowType);
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUI.DragWindow ();
	}

	public abstract void ConnectToNode (Node otherNode);
	public virtual void DrawRelationArrows (Action<Vector3, Vector3> drawCallback) {}

	public Vector2 nodeConnectionPoint(Vector2 otherPos){
		Vector2 myPos = windowRect.center;
		float xDiff = otherPos.x - myPos.x;
		float yDiff = otherPos.y - myPos.y;

		if (Mathf.Abs (xDiff) > Mathf.Abs (yDiff)) {
			int extentMultiplier = (xDiff > 0) ? 1 : -1;
			myPos.x += windowRect.width / 2 * extentMultiplier;
		}
		else {
			int extentMultiplier = (yDiff > 0) ? 1 : -1;
			myPos.y += windowRect.height / 2 * extentMultiplier;
		}

		return myPos;
	}

	protected void LinkMeToOtherNode(Node otherNode){
		otherNode.onNodeDestroyed += OnLinkedNodeDestroyed;		
	}
	public virtual void DestroyNode(){//make sure that all nodes attached to me remove this link
		if(onNodeDestroyed != null){
			onNodeDestroyed (this);
		}
	}
	protected abstract void OnLinkedNodeDestroyed (Node otherNode);//this can be subscribed to the onNodeDestroyed event
}
