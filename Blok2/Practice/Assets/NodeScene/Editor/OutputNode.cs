using UnityEngine;
using UnityEditor;

public class OutputNode : Node {
	private string result = "";
	private BaseInputNode inputNode;

	private Rect inputNodeRect;

	public OutputNode(){
		windowTitle = "Output Node";
		hasInputs = true;
	}

	public override void DrawWindow (){
		base.DrawWindow ();
		Event e = Event.current;
		string input1 = "None";

		if (inputNode) {
			input1 = inputNode.getResult ().ToString();
		}

		GUILayout.Label ("Input 1: " + input1);

		if (e.type == EventType.Repaint) {
			inputNodeRect = GUILayoutUtility.GetLastRect ();
		}

		GUILayout.Label ("Result: " + result);
	}

	//draw curve from input node to this node:
	public override void DrawCurves(){
		if (inputNode) {
			Rect rect = windowRect;
			rect.x += inputNodeRect.x;
			rect.y += inputNodeRect.y + inputNodeRect.height / 2;
			rect.width = 1;
			rect.height = 1;

			NodeEditor.DrawNodeCurve (inputNodeRect, rect);
		}
	}

	public override void NodeDeleted(Node node){
		if (node.Equals (inputNode)) {
			inputNode = null;
		}
	}

	//if a click happens inside my inputNode:
	public override BaseInputNode ClickedOnInput(Vector2 pos){
		BaseInputNode retVal = null;

		pos.x -= windowRect.x;
		pos.y -= windowRect.y;

		if(inputNodeRect.Contains(pos)){
			retVal = inputNode;
			inputNode = null;
		}

		return retVal;
	}

	public override void SetInput (BaseInputNode input, Vector2 clickPos){
		clickPos.x -= windowRect.x;
		clickPos.y -= windowRect.y;

		if(inputNodeRect.Contains(clickPos)){
			inputNode = input;
		}
	}
}
