using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildingManager))]
public class BuildingManagerInspector : Editor {

	public override void OnInspectorGUI (){
		serializedObject.Update ();
		DrawPropertiesExcluding (serializedObject, "m_Script", "bridgeSize", "houseSize");

		BuildingManager building = (BuildingManager)target; 
		SerializedProperty size = serializedObject.FindProperty (building.SizePropertyPath ());

		GUILayout.Label ("Realtime variables:");

		//building.buildingSize = EditorGUILayout.Slider ("Size", building.buildingSize, 0.5f, 3.5f);
		building.ResizeBuildings ();

		serializedObject.ApplyModifiedProperties ();
	}
}
