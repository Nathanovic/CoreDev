using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Bridge))]
public class BridgeInspector : Editor {

	public override void OnInspectorGUI (){
		serializedObject.Update ();
		//base.OnInspectorGUI ();
		DrawPropertiesExcluding (serializedObject, "m_Script");
		serializedObject.ApplyModifiedProperties ();
	}
}
