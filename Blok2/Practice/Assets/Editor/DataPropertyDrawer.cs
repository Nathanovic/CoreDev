using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(Data))]
public class DataPropertyDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		SerializedProperty value = property.FindPropertyRelative ("value");
		SerializedProperty open = property.FindPropertyRelative ("open");

		float lineHeight = EditorGUIUtility.singleLineHeight;
		position.height = lineHeight;

		open.boolValue = EditorGUI.Foldout (position, open.boolValue, "Data");

		if (open.boolValue) {
			position.y += lineHeight;
			EditorGUI.indentLevel++;

			EditorGUI.PropertyField (position, value);
			EditorGUI.indentLevel--;
		}
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label){
		SerializedProperty open = property.FindPropertyRelative ("open");
		float lineHeight = EditorGUIUtility.singleLineHeight;
		if (open.boolValue) {
			return 2 * lineHeight;
		}
		return lineHeight;
	}
}
