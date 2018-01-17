using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class ConditionNode : Node {

	[SerializeField]private string objectTag;
	[SerializeField]private TrueFalseCondition trueFalse;//kan de voorwaarde inverten

	[SerializeField]private StateNode nextStateNode;
	[SerializeField]public Condition ConditionModel { 
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

	[SerializeField]private bool showTagField;

	public void InitConditionNode(){
		windowRect = new Rect(300,150,300,10);
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
		string conditionTypeName = dropdownOptions [selectedDropdownIndex] + "_C";
		Type currentConditionType = Assembly.GetExecutingAssembly ().GetType (conditionTypeName);
		ConditionModel = ScriptableObject.CreateInstance(currentConditionType) as Condition;
		windowTitle = "Condition (" + ConditionModel.type.ToString() + ")";

		AssetDatabase.AddObjectToAsset (ConditionModel, linkedFSM);
		//Undo.RecordObject (conditionModel, "created condition class in Node");

		showTagField = ConditionModel.requireTags;
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
		ConditionModel.InitNextState (null);
		nextStateNode = null;
	}
	#endregion

	public override void PrepareModel(out bool succes) {//used when data should be saved (from FSMData)
		if (ConditionModel.requireTags) {
			ConditionModel.PassTags (objectTag);
		}

		if (nextStateNode == null) {
			succes = false;
		}
		else {
			ConditionModel.InitNextState (nextStateNode.StateModel);
			succes = true;
		}
	}
}
