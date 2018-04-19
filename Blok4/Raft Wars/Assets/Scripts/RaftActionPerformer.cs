using UnityEngine;

//contains the base for performing an action with the raft
public abstract class RaftActionPerformer : MonoBehaviour {

	private Player playerScript;

	private void Start(){
		playerScript = GetComponent<Player> ();
	}

	public abstract void EvaluateInput (out bool succes);

	protected void FinishAction(){
		playerScript.OnActionFinished ();
	}
}
