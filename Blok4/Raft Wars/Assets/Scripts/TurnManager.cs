using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//communicates with the players about whether they are allowed to do an action
//the active player can only be updated on the server-side!
public class TurnManager : NetworkBehaviour {

	public static TurnManager instance;

	private const float waitTime = 1f;
	private int activePlayerIndex;

	[SyncVar]
	private List<int> players;

	public int myNetID;

	public delegate void OnActivePlayerChanged(int playerNetID);
	public event OnActivePlayerChanged onActivePlayerChanged;

	private void Awake(){
		instance = this;
		players = new List<int> ();
	}

	public void OnConnectedToServer(){
		myNetID = GetComponent<NetworkIdentity> ().netId;
		if (isServer) {
			players.Add (myNetID);
		} 
		else {
			CmdOnPlayerJoined (myNetID);	
		}
	}

	private void Start () {
		//choose first player random
		int rndmPlayerIndex = Random.Range (0, players.Count);
		activePlayerIndex = rndmPlayerIndex;
	}

	public void NextTurn(){
		if (isServer) {
			RpcDoNextTurn ();
		} 
		else {
			CmdDoNextTurn ();
		}
	}

	[Command]
	private void CmdOnPlayerJoined(int playerNetID){
		players.Add (playerNetID);
	}

	[Command]
	private void CmdDoNextTurn(){
		activePlayerIndex++;
		if (activePlayerIndex == players.Count) {
			activePlayerIndex = 0;
		}

		Player activePlayer = players [activePlayerIndex];
		activePlayer.GrantActionPermission ();

		RpcDoNextTurn ();
	}

	[ClientRpc]
	private void RpcDoNextTurn(int newActivePlayer){
		onActivePlayerChanged (newActivePlayer);
	}
}
