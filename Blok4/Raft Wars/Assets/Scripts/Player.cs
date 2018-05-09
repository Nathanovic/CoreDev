using UnityEngine;

//this script is used to get the structure the action handling of the player 
//whether the player is allowed to do an action is communicated from the ActionManager
[RequireComponent(typeof(RaftMovement))]
public class Player : MonoBehaviour {

	private bool canDoAction;

	public PlayerInfo myInfo;

	private RaftActionPerformer[] raftActions;

	private void Start(){
		raftActions = GetComponents<RaftActionPerformer> ();
	}

	//allow the player to do an action
	public void GrantActionPermission(){
		canDoAction = true;
	}

	//check what action we will do
	private void Update(){
		if (!canDoAction)
			return;

		foreach (RaftActionPerformer actionPerformer in raftActions) {
			bool actionStarted = false;
			actionPerformer.EvaluateInput (out actionStarted);
			if (actionStarted) {
				canDoAction = false;
				break;
			}
		}
	}

	public void OnActionStarted(){
		canDoAction = false;
	}

	public void OnActionFinished(){
		TurnManager.instance.NextTurn ();
	}
}

[System.Serializable]
public struct PlayerInfo{
	public string playerName;
	public Color playerColor;
}
