using UnityEngine;
using UnityEditor;
using AI_UtilitySystem;

//this script ensures that we only draw the properties that are relevant for what the player wants 
[CustomEditor(typeof(DecisionFactor))]
[CanEditMultipleObjects]
public class DecisionFactorEditor : Editor {

	public override void OnInspectorGUI () {
		serializedObject.Update ();
		DrawPropertiesExcluding (serializedObject, "m_Script", "minStatValue", "maxStatValue", "statCurve");

		SerializedProperty minStatValueProp = serializedObject.FindProperty ("minStatValue");
		SerializedProperty maxStatValueProp = serializedObject.FindProperty ("maxStatValue");
		FactorDecider factorDecider = (FactorDecider)serializedObject.FindProperty ("factorSolver").enumValueIndex;

		if (factorDecider == FactorDecider.Greater) {
			GUILayout.Label ("stat greater than:");
			EditorGUILayout.PropertyField (minStatValueProp, new GUIContent ("threshold value:"));
		}
		else if (factorDecider == FactorDecider.Less) {
			GUILayout.Label ("stat less than:");
			EditorGUILayout.PropertyField (maxStatValueProp, new GUIContent ("threshold value:"));
		}
		else {
			bool rndm = (factorDecider == FactorDecider.Random);
			string valueLabel = rndm ? "random between:" : "stat between:";
			GUILayout.Label (valueLabel);
			GUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField (minStatValueProp, GUIContent.none);
			EditorGUILayout.PropertyField (maxStatValueProp, GUIContent.none);
			GUILayout.EndHorizontal ();

			if (!rndm) {
				SerializedProperty curveProp = serializedObject.FindProperty ("statCurve");
				EditorGUILayout.PropertyField (curveProp, GUILayout.Height (70));
			}
		}

		serializedObject.ApplyModifiedProperties ();
	}
}
