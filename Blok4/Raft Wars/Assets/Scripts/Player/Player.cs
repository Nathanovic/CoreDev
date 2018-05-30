using UnityEngine;
using UnityEngine.Networking;

//this script is used to get the structure the action handling of the player 
//whether the player is allowed to do an action is communicated from the ActionManager
[RequireComponent(typeof(RaftMovement))]
[RequireComponent(typeof(Health))]
public class Player : NetworkBehaviour {

	public UserStats userInfo;
	private bool canDoAction;
	public bool alive{ get; private set; }

	private RaftActionPerformer[] raftActions;

	#region initialization handling
	private void Start(){
		if (isLocalPlayer) {
			raftActions = GetComponents<RaftActionPerformer> ();
			string uName = GameManager.instance.userName;
			int uID = GameManager.instance.userID;
			UserStats newUserInfo = GetComponent<UserStats> ();
			newUserInfo.RegisterSelf (uID, uName);
			userInfo = newUserInfo;
		}

		if (isServer) {
			alive = true;
			userInfo = GetComponent<UserStats> ();
			TurnManager.instance.InitializeServerPlayer (this);
			GetComponent<Health> ().onDie += ServerPlayerDied;
		}
	} 

	[ClientRpc]//client initialization:
	public void RpcInitializeRaft(int playerID, Color userColor){ 
		if (isLocalPlayer) {
			transform.name = "Raft_Local";//useful for debugging
		}

		GetComponent<SpriteRenderer> ().color = Color.Lerp(userColor, Color.white, 0.9f);
		transform.Translate (transform.right * playerID);
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

	private void ServerPlayerDied(){
		alive = false;
		GetComponent<BoxCollider2D> ().enabled = false;
		TurnManager.instance.ServerPlayerDied (this);
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
}
