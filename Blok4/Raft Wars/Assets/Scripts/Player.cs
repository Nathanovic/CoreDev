using UnityEngine;
using UnityEngine.Networking;

//this script is used to get the structure the action handling of the player 
//whether the player is allowed to do an action is communicated from the ActionManager
[RequireComponent(typeof(RaftMovement))]
public class Player : NetworkBehaviour {

	public User userInfo;
	private bool canDoAction;

	private RaftActionPerformer[] raftActions;

	#region initialization handling
	private void Start(){
		if (isLocalPlayer) {
			raftActions = GetComponents<RaftActionPerformer> ();
			userInfo = NetworkManager.singleton.gameObject.GetComponent<User> ();
			User newUserInfo = GetComponent<User> ();
			newUserInfo.RegisterSelf (userInfo.userID, userInfo.userName);
			Destroy (userInfo);
			userInfo = newUserInfo;
		}

		if (isServer) {
			userInfo = GetComponent<User> ();
			TurnManager.instance.InitializeServerPlayer (this);
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
