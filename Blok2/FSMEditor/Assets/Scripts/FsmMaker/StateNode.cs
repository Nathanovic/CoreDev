using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class StateNode : Node {

	private string stateName;

	public State stateModel { 
		get { 
			return linkedFSM.states [linkedModelIndex];
		}
		set{ 
			if (linkedModelIndex == -1) {
				linkedModelIndex = linkedFSM.states.Count;
				linkedFSM.states.Add (value);
			}
			else {
				linkedFSM.states [linkedModelIndex] = value;
			}
		}
	}

	public List<ConditionNode> fromStateConditionNodes;

	public StateNode(){
		windowRect = new Rect (50, 150, 300, 50);
		windowTitle = "state node";
		windowType = "state";
		dropdownDescription = "State: ";
		fromStateConditionNodes = new List<ConditionNode> (3);
	}

	#region GUI
	protected override void DoMyWindow (int windowID) {
		base.DoMyDropdown ();
		base.DoMyWindow (windowID);
	}

	public override void DrawRelationArrows (Action<Vector3, Vector3> drawCallback) {
		for(int i = 0; i < fromStateConditionNodes.Count; i ++){
			Vector2 statePos = fromStateConditionNodes[i].nodeConnectionPoint (windowRect.center);
			Vector2 myPos = nodeConnectionPoint (fromStateConditionNodes[i].windowRect.center);
			drawCallback (myPos, statePos);
		}
	}
	#endregion

	#region DataHandling
	protected override void CreateModelFromIndex () {
		string stateTypeName = dropdownOptions [selectedDropdownIndex];
		Type currentState = Assembly.GetExecutingAssembly ().GetType (stateTypeName);
		//stateModel = Activator.CreateInstance (currentState) as State;
		stateModel = ScriptableObject.CreateInstance(currentState) as State;
		Undo.RecordObject (stateModel, "created state class in Node");
	}

	public override void ConnectToNode (Node otherNode) {
		if (otherNode.GetType () == typeof(ConditionNode)) {
			ConditionNode fromStateCondition = (ConditionNode)otherNode;
			if (!fromStateConditionNodes.Contains (fromStateCondition)) {
				LinkMeToOtherNode (otherNode);//used when the other node is destroyed
				fromStateConditionNodes.Add (fromStateCondition);
			}
		}
	}

	public override void DestroyNode(){
		base.DestroyNode ();
		linkedFSM.invalidStates.Add (linkedModelIndex);
	}

	protected override void OnLinkedNodeDestroyed (Node otherNode) {
		ConditionNode other = (ConditionNode)otherNode;
		fromStateConditionNodes.Remove (other);
	}
	#endregion

	public State GetStateFromNode (){
		List<Condition> fromStateConditions = new List<Condition> (fromStateConditionNodes.Count);
		for (int i = 0; i < fromStateConditions.Count; i++) {
			fromStateConditions.Add (fromStateConditionNodes[i].conditionModel);
		}

		stateModel.InitConditions (fromStateConditions.ToArray());
		return stateModel;
	}
}