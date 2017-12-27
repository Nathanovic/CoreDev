using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ComparisonNode : BaseInputNode {

	private BaseInputNode input1;
	private Rect input1Rect;

	private BaseInputNode input2;
	private Rect input2Rect;

	private ComparisonType comparisonType;
	private string compareText = "";

	public enum ComparisonType{
		Greater,
		Less,
		Equal
	}

	public ComparisonNode(){
		windowTitle = "Comparison Node";
		hasInputs = true;
	}

	public override void DrawWindow(){
		base.DrawWindow ();

		Event e = Event.current;
		comparisonType = (ComparisonType)EditorGUILayout.EnumPopup ("Comparison Type", comparisonType);


	}
}
