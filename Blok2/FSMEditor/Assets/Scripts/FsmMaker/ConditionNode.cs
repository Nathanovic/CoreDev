using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class ConditionNode : Node {

	private string[] stateOptions;
	private int previousStateIndex;
	private int selectedStateIndex;
	private string objectTag;
	private TrueFalseCondition trueFalse;//kan de voorwaarde inverten

	Condition conditionModel;

	public ConditionNode(){
		windowRect = new Rect(300,150,300,50);
		windowTitle = "condition node";
		windowType = "object condition";
		objectTag = "Player";
		trueFalse = TrueFalseCondition.True;
	}

	public void Init(string[] states){
		stateOptions = states;
	}

	protected override void DoMyWindow (int windowID) {
		selectedStateIndex = EditorGUILayout.Popup ("Action: ", selectedStateIndex, stateOptions);
		EvaluateStateIndex ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Other tag: ");
		objectTag = GUILayout.TextField (objectTag, 10, GUILayout.MaxWidth(140));
		GUILayout.EndHorizontal ();
		trueFalse = (TrueFalseCondition)EditorGUILayout.EnumPopup ("Condition: ", trueFalse);

		base.DoMyWindow (windowID);
	}
		
	void EvaluateStateIndex(){
		if (selectedStateIndex != previousStateIndex) {
			previousStateIndex = selectedStateIndex;
			string conditionType = stateOptions [selectedStateIndex];
			Type iets = Assembly.GetExecutingAssembly ().GetType (conditionType);
			conditionModel = Activator.CreateInstance (iets) as Condition;
			Debug.Log (iets.Name);
			Debug.Log ("condition model: " + conditionModel);
		}
	}

	private enum TrueFalseCondition{
		True,
		False
	}

	private Dictionary<string, Condition> conditionDictionary = new Dictionary<string, Condition>();

	public Condition GetConditionFromNode(){//used when data should be saved (from FSMData)
		if (conditionModel.requireTags) {
			conditionModel.PassTags (objectTag);
		}
		return conditionModel;
	}
}
