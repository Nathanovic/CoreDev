using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class ConditionNode : Node {

	private string objectTag;
	private TrueFalseCondition trueFalse;//kan de voorwaarde inverten

	private StateNode nextStateNode;

	public Condition conditionModel { 
		get {
			return linkedFSM.conditions[linkedModelIndex];
		}
		private set { 
			if (linkedModelIndex == -1) {
				linkedModelIndex = linkedFSM.conditions.Count;
				linkedFSM.conditions.Add (value);
			}
			else {
				linkedFSM.conditions [linkedModelIndex] = value;
			}
		}
	}

	private bool showTagField;

	public ConditionNode(){
		windowRect = new Rect(300,150,300,10);
		windowTitle = "condition node";
		windowType = "object condition";
		objectTag = "Player";
		trueFalse = TrueFalseCondition.True;
		dropdownDescription = "Action: ";

		showTagField = false;
	}

	#region GUI
	protected override void DoMyWindow (int windowID) {
		base.DoMyDropdown ();
		if (showTagField) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Other tag: ");
			objectTag = GUILayout.TextField (objectTag, 10, GUILayout.MaxWidth (140));
			GUILayout.EndHorizontal ();
		}
		trueFalse = (TrueFalseCondition)EditorGUILayout.EnumPopup ("Condition: ", trueFalse);

		base.DoMyWindow (windowID);
	}

	public override void DrawRelationArrows (Action<Vector3, Vector3> drawCallback) {
		if (nextStateNode != null) {
			Vector2 statePos = nextStateNode.nodeConnectionPoint (windowRect.center);
			Vector2 myPos = nodeConnectionPoint (nextStateNode.windowRect.center);
			drawCallback (myPos, statePos);
		}
	}
	#endregion

	#region DataHandling
	protected override void CreateModelFromIndex(){
		string conditionTypeName = dropdownOptions [selectedDropdownIndex];
		Type currentConditionType = Assembly.GetExecutingAssembly ().GetType (conditionTypeName);
		//conditionModel = Activator.CreateInstance (currentConditionType) as Condition;
		conditionModel = ScriptableObject.CreateInstance(currentConditionType) as Condition;
		Undo.RecordObject (conditionModel, "created condition class in Node");

		showTagField = conditionModel.requireTags;
		windowRect.height = 10f;//nodig om de height opnieuw flexibel te berekenen
	}

	private enum TrueFalseCondition{
		True,
		False
	}

	public override void ConnectToNode (Node otherNode) {
		if (otherNode.GetType () == typeof(StateNode)) {
			LinkMeToOtherNode (otherNode);//used when the other node is destroyed
			nextStateNode = (StateNode)otherNode;
		}
	}

	public override void DestroyNode(){
		base.DestroyNode ();
		linkedFSM.invalidConditions.Add (linkedModelIndex);
	}

	protected override void OnLinkedNodeDestroyed (Node otherNode) {
		conditionModel.InitNextState (null);
		nextStateNode = null;
	}
	#endregion

	public Condition GetConditionFromNode(){//used when data should be saved (from FSMData)
		if (conditionModel.requireTags) {
			conditionModel.PassTags (objectTag);
		}
		if (nextStateNode == null) {
			Debug.LogWarning ("this FSM will not work correctly since one ore more conditions are not linked to a state");
		}
		else {
			conditionModel.InitNextState (nextStateNode.stateModel);
		}
		return conditionModel;
	}
}
