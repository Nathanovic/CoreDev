using UnityEngine;
using UnityEditor;

public class InputNode : BaseInputNode {

	private InputType inputType;

	public enum InputType{
		Number,
		Randomization
	}

	private float randomFrom = 0;
	private float randomTo = 0;

	private float inputValue = 0;

	public InputNode(){
		windowTitle = "Input Node";
	}

	public override void DrawWindow (){
		base.DrawWindow ();
		inputType = (InputType)EditorGUILayout.EnumPopup ("Input type: ", inputType);

		if (inputType == InputType.Number) {
			inputValue = EditorGUILayout.FloatField ("Value", inputValue);
			inputValue = RoundedFloat (inputValue);
		} else {
			EditorGUILayout.BeginHorizontal ();
			randomFrom = EditorGUILayout.FloatField ("From", randomFrom);
			randomTo = EditorGUILayout.FloatField ("To", randomTo);
			EditorGUILayout.EndHorizontal ();

			if (GUILayout.Button ("Calculate Random")) {
				float rndmFloat = Random.Range (randomFrom, randomTo);
				inputValue = RoundedFloat (rndmFloat);
				Debug.Log ("Calculate, value = " + inputValue);
			}
		}
	}

	public override void DrawCurves(){
	}

	public override float getResult (){
		return inputValue;
	}
}
