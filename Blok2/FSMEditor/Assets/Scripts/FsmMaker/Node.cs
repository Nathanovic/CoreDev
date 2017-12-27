using UnityEngine;
using UnityEditor;

public class Node : ScriptableObject {
	//scriptableObject is needed to have functions like OnDestroy

	protected Rect windowRect;
	protected string windowTitle;
	protected string windowType;
	protected const int MAX_STRING_LENGTH = 15;

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
}
