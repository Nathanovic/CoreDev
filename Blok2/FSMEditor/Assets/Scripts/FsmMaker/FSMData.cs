using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMData : ScriptableObject {

	[HideInInspector]public EditorData editorData = new EditorData();

	public string unitType = "default";

	public List<Condition> conditions = new List<Condition>();
	public List<State> states = new List<State> ();

	public void PassData(EditorData data){
		editorData = data;
		unitType = data.unitType;

		foreach (Node node in data.nodeWindows) {
			if (node.GetType () == typeof(StateNode)) {
				StateNode stateNode = (StateNode)node;
				states.Add (stateNode.GetStateFromNode ());
			} else if (node.GetType () == typeof(ConditionNode)) {
				ConditionNode conditionNode = (ConditionNode)node;
				conditions.Add (conditionNode.GetConditionFromNode());
			}
		}
	}
}