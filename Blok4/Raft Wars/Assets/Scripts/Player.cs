using UnityEngine;
using UnityEngine.Networking;

//this script is used to get the structure the action handling of the player 
//whether the player is allowed to do an action is communicated from the ActionManager
[RequireComponent(typeof(RaftMovement))]
public class Player : NetworkBehaviour {

	public PlayerInfo myInfo; 
	private bool canDoAction{
		get{ 
			return myInfo.canDoAction;
		}
		set{ 
			myInfo.canDoAction = value;
		}
	}

	private RaftActionPerformer[] raftActions;

	#region initialization handling
	private void Start(){
		raftActions = GetComponents<RaftActionPerformer> ();
		if (isServer) {
			TurnManager.instance.InitializeServerPlayer (this);
		}
	} 

	public override void OnStartLocalPlayer () {
		TurnManager.instance.InitializeLocalPlayer (this);
	}

	[ClientRpc]//client initialization:
	public void RpcInitializeRaft(int raftID){ 
		bool playerIsServer = (raftID == 0);
		transform.name = playerIsServer ? "Raft_Server" : "Raft_Client";
		myInfo.playerName = playerIsServer ? "ServerRaft" : "ClientRaft";
		myInfo.playerColor = playerIsServer ? Color.blue : Color.red;
		GetComponent<SpriteRenderer> ().color = myInfo.playerColor;
		transform.Translate (transform.right * raftID);

		Debug.Log ("initialize raft with id: " + raftID);
	}
	#endregion

	private void Update(){
		if (!isLocalPlayer || !canDoAction)
			return;

		//check what action we will do
		foreach (RaftActionPerformer actionPerformer in raftActions) {
			bool actionStarted = false;
			actionPerformer.EvaluateInput (out actionStarted);
			if (actionStarted) {
				canDoAction = false;
				break;
			}
		}
	}

	[ClientRpc]
	public void RpcGrantActionPermission(){
		GrantActionPermission ();
	}

	//allow the player to do an action
	public void GrantActionPermission(){
		canDoAction = true;
	}

	public void OnActionStarted(){
		canDoAction = false;
	}

	public void OnActionFinished(){
		TurnManager.instance.NextTurn ();
	}
}

[System.Serializable]
public class PlayerInfo{
	public int raftID;//is only updated on the server
	public NetworkInstanceId netID;//is only updated on the server
	public string playerName;
	public Color playerColor;
	public bool canDoAction;//public QQQ
}
