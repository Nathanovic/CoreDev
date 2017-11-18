using UnityEngine;
using UnityEditor;

public class CharacterCreatorEditor : EditorWindow {

	public string characterName;
	public Color color;

	[MenuItem("Custom/CharacterCreator")]
	public static void ShowWindow(){
		EditorWindow.GetWindow<CharacterCreatorEditor> ("Character creator");//opens up a window for us, and makes sure that there can't be more than 1 window
	}

	void OnGUI(){
		GUILayout.Label ("Character Creator", EditorStyles.boldLabel);
		characterName = EditorGUILayout.TextField ("Name", characterName);

		color = EditorGUILayout.ColorField ("Color", color);

		if (GUILayout.Button ("Create random")) {
			Debug.Log ("Create random character, named: " + characterName);
			Colorize ();
		}
	}

	void Colorize(){
		foreach (Transform t in Selection.transforms) {
			Renderer r = t.GetComponent<Renderer> ();
			if (r != null) {
				r.sharedMaterial.color = color;
			}
		}
	}
}
