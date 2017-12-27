using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Bridge))]
public class BridgeInspector : Editor {

	public override void OnInspectorGUI (){
		serializedObject.Update ();
		//base.OnInspectorGUI ();
		DrawPropertiesExcluding (serializedObject, "m_Script", "mySize");

		Bridge b = (Bridge)target; 

		GUILayout.BeginHorizontal ();
		if(GUILayout.Button("Generate color")){
			b.ColorMe();
		}
		if(GUILayout.Button("Reset color")){
			b.ResetMe();
		}
		GUILayout.EndHorizontal ();

		serializedObject.ApplyModifiedProperties ();
	}
}
