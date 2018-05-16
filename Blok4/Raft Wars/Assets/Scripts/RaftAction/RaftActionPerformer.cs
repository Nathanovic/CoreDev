using UnityEngine;
using UnityEngine.Networking;

//contains the base for performing an action with the raft
public abstract class RaftActionPerformer : NetworkBehaviour {

	private Player playerScript;
	protected Animator anim;

	private void Start(){
		playerScript = GetComponent<Player> ();
		anim = GetComponent<Animator> ();
	}

	public abstract void EvaluateInput (out bool succes);

	protected void FinishAction(){
		playerScript.OnActionFinished ();
	}
}
