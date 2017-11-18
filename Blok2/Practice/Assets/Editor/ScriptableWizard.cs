using UnityEngine;
using System.Collections;
using UnityEditor;

public class RinceWizard : ScriptableWizard {
	public float myRange = 50;

	[MenuItem("Some menu/RinceWizard")]

	public static void Open(){
		ScriptableWizard.DisplayWizard<RinceWizard> ("Create Rince", "Create", "Apply");
	}

	//write code to modify the wizard:
	protected override bool DrawWizardGUI(){
		return base.DrawWizardGUI ();
	}

	//this gets called when you hit the create button:
	void OnWizardCreate(){
		Debug.Log ("BOom, created RinceWizard!");
	}
}

public class SomeEditor : EditorWindow{
	[MenuItem("Some menu/Editor Window")]

	public static void Open(){
		SomeEditor.GetWindow<SomeEditor> ();
	}

	public void OnGUI(){
		GUILayout.Label ("Editor window :) :");
		GUILayout.Label ("Hellow world, this is a editor window");
	}
}