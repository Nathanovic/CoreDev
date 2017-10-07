using UnityEngine;
using System.Collections;

public class Player : Character {

	protected override void Start () {
		base.Start ();
		//print ("player start");
	}

	private void Update(){
		UpdateFSM ();
	}
		
	//abstract implementations
	public override void ControlMe(){
		Vector2 inputInstruction = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		inputInstruction.Normalize ();

		if(GetFrontCollider(inputInstruction) == null){
			MoveMe (inputInstruction);
		}
	}
}
