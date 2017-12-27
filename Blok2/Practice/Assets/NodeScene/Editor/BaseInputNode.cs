using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class BaseInputNode : Node {

	//this returns the result of the node:
	public virtual float getResult(){
		return 0f;
	}

	protected float RoundedFloat(float value){
		int intValue = (int)(value * 10);
		return (float)intValue / 10f;
	}

	protected void Foo <T> (T myEnum) {
		//probeer casten naar enum
	}

	void DrawInputTitle(BaseInputNode inputNode, int id){
		string inputTitle = "None";
		if (inputNode) {
			inputTitle = inputNode.getResult ().ToString();
		}
		GUILayout.Label ("Input " + id.ToString ());
	}

	public override void DrawCurves(){
		
	}
}
