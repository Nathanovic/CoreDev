using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestEditor : EditorWindow {

	private Rect windowRect = new Rect (50, 50, 200, 160);

	[MenuItem("Some menu/TestEditor")]
	static void ShowEditor(){
		TestEditor editor = (TestEditor)EditorWindow.GetWindow (typeof(TestEditor));
		editor.Show ();
	}

	void OnGUI(){
		GUILayout.Label ("Label!");

		BeginWindows ();
		windowRect = GUILayout.Window (0, windowRect, DoMyWindow, "WindowTitle");
		EndWindows ();
	}

	void DoMyWindow(int id){
		if (GUILayout.Button("Please click me a lot"))
			Debug.Log("Got a click");

		GUI.DragWindow ();
	}
}
