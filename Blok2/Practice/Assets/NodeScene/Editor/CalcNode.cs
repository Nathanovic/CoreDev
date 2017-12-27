using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CalcNode : BaseInputNode {

	private BaseInputNode input1;
	private Rect input1Rect;

	private BaseInputNode input2;
	private Rect input2Rect;

	private CalculationType calculationType;
	public enum CalculationType{
		Addition,
		Substraction,
		Multiplication,
		Division
	}

	public CalcNode(){
		windowTitle = "Calculation Node";
		hasInputs = true;
	}

	public override void DrawWindow ()
	{
		base.DrawWindow ();
		base.DrawCurves ();
		calculationType = (CalculationType)EditorGUILayout.EnumPopup ("Calculation type: ", calculationType);

		DrawInputTitle (input1, 1, ref input1Rect);
		DrawInputTitle (input2, 2, ref input2Rect);
	}
	void DrawInputTitle(BaseInputNode inputNode, int id, ref Rect inputRect){
		string inputTitle = "None";
		if (inputNode) {
			inputTitle = inputNode.getResult ().ToString();
		}
		GUILayout.Label ("Input " + id.ToString ());
		if (Event.current.type == EventType.Repaint) {
			inputRect = GUILayoutUtility.GetLastRect ();
		}
	}

	public override void DrawCurves (){
		DrawCurvesForInputNode (input1Rect, input1);
		DrawCurvesForInputNode (input2Rect, input2);
	}
	private void DrawCurvesForInputNode(Rect targetRect, BaseInputNode inputNode){
		if (inputNode) {
			Rect rect = windowRect;
			rect.x += targetRect.x;
			rect.y += targetRect.y + targetRect.height / 2;
			rect.width = 1;
			rect.height = 1; 

			NodeEditor.DrawNodeCurve (inputNode.windowRect, rect);
		}
	}

	public override float getResult (){
		float input1Value = inputResult (input1);
		float input2Value = inputResult (input2);

		float resultValue = 0;

		switch (calculationType) {
		case CalculationType.Addition:
			resultValue = input1Value + input2Value;
			break;
		case CalculationType.Division:
			float divValue = input1Value + input2Value;
			resultValue = RoundedFloat (divValue);
			break;
		case CalculationType.Multiplication:
			resultValue = input1Value * input2Value;
			break;
		case CalculationType.Substraction:
			resultValue = input1Value - input2Value;
			break;
		}

		return resultValue;
	}
	float inputResult(BaseInputNode inputNode){
		float res = 0;
		if (inputNode) {
			res = inputNode.getResult ();
		}

		return res;
	}

	public override void SetInput(BaseInputNode input, Vector2 clickPos){
		clickPos.x -= windowRect.x;
		clickPos.y -= windowRect.y;

		if(input1Rect.Contains(clickPos)){
			input1 = input;
		}else if(input2Rect.Contains(clickPos)){
			input2 = input;
		}
	}

	public override BaseInputNode ClickedOnInput(Vector2 pos){
		BaseInputNode retVal = null;

		pos.x -= windowRect.x;
		pos.y -= windowRect.y;

		if (input1Rect.Contains (pos)) {
			retVal = input1;
			input1 = null;
		} else if (input2Rect.Contains (pos)) {
			retVal = input2;
			input2 = null;
		}

		return retVal;
	}

	public override void NodeDeleted (Node node){
		if (node.Equals (input1)) {
			input1 = null;
		} else if (node.Equals (input2)) {
			input2 = null;
		}
	}
}
