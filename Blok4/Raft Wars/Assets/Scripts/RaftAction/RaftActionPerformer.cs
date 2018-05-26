using UnityEngine;
using UnityEngine.Networking;

//contains the base for performing an action with the raft
public abstract class RaftActionPerformer : NetworkBehaviour {

	protected Animator anim;

	protected virtual void Start(){
		anim = GetComponent<Animator> ();
	}

	public abstract void EvaluateInput (out bool succes);

	protected void ServerOnActionFinished(){
		TurnManager.instance.ServerNextTurn ();
	}
}
