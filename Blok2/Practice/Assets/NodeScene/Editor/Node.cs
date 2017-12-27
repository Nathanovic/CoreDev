using UnityEngine;
using UnityEditor;

public abstract class Node : ScriptableObject {

	public Rect windowRect;
	protected bool hasInputs = false;
	public string windowTitle;

	public virtual void DrawWindow(){ 
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Title:");
		windowTitle = EditorGUILayout.TextField(windowTitle);
		EditorGUILayout.EndHorizontal ();
	}

	public abstract void DrawCurves ();

	public virtual void SetInput(BaseInputNode input, Vector2 clickPos){
		
	}

	public virtual void NodeDeleted(Node node){
		
	}

	//return the input that was clicked:
	public virtual BaseInputNode ClickedOnInput(Vector2 pos){
		return null;
	}
}

public enum State{
	idle,
	attack,
	die
}
