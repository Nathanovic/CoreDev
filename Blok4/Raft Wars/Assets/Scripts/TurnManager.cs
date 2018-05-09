using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//communicates with the players about whether they are allowed to do an action
//the active player can only be updated on the server-side!
public class TurnManager : NetworkBehaviour {

	public static TurnManager instance;
	private Player playerScript;

	private const float waitTime = 1f;
	private int activePlayerIndex;
	private List<NetworkInstanceId> players;
	public NetworkInstanceId myNetID;

	public delegate void OnActivePlayerChanged(NetworkInstanceId playerNetID);
	public event OnActivePlayerChanged onActivePlayerChanged;

	private void Awake(){
		instance = this;
		players = new List<NetworkInstanceId> ();
	}

	public void OnConnectedToServer(){
		playerScript = GetComponent<Player> ();
		myNetID = GetComponent<NetworkIdentity> ().netId;//used to evaluate who's turn it is
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

	//can only be ran on the server
	public void NextTurn(){
		if (isServer) {
			RpcDoNextTurn (myNetID);
		} 
	}

	[Command]
	void CmdOnPlayerJoined(NetworkInstanceId newPlayerID){
		
	}

	[ClientRpc]
	private void RpcDoNextTurn(){
		activePlayerIndex++;
		if (activePlayerIndex == players.Count) {
			activePlayerIndex = 0;
		}

		onActivePlayerChanged (newActivePlayer);
	}



}   
 