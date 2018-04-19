using UnityEngine;

//this script does not communicate directly with other RaftCombat scripts
//this is possible since the damage is done by triggers of projectiles
public class RaftCombat : RaftActionPerformer {

	public override void EvaluateInput (out bool succes) {
		if (Input.GetKeyUp (KeyCode.Space)) {
			Debug.Log (transform.name + " fires projectile!");
			succes = true;
			FinishAction ();
		} 
		else {
			succes = false;
		}
	}

	//check if we are being hit
	private void OnTriggerEnter2D(){
		
	}
}
