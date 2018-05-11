using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//communicates with the players about whether they are allowed to do an action
//the active player can only be updated on the server-side!
//this script however, is active on all clients
public class TurnManager : NetworkBehaviour {

	public static TurnManager instance;

	private const float waitTime = 1f;
	private int activePlayerIndex;
	private Player playerScript;
	private List<Player> players;
	private List<NetworkInstanceId> playerIDs;//not used...
	private NetworkInstanceId myNetID; 

	public int requiredPlayerCount = 2;
	private int playerCount;

	private void Awake(){		
		instance = this;
		playerIDs = new List<NetworkInstanceId> (); 
	}

	#region connection handling
	public void InitializeLocalPlayer (Player player) {
		playerScript = player;
		myNetID = playerScript.netId;//used to evaluate who's turn it is		
	}

	//only called on the server
	public void InitializeServerPlayer(Player player){ 
		playerIDs.Add (player.netId);
		players.Add (player);
		player.myInfo.raftID = playerCount;
		player.myInfo.netID = player.netId;

		playerCount++;

		if (playerCount == requiredPlayerCount) {
			playerScript.GrantActionPermission ();

			foreach (Player p in players) {
				p.RpcInitializeRaft (p.myInfo.raftID);
			}
		}
	}
	#endregion

	#region turn handling
	public void ServerNextTurn(){
		Debug.Log ("server next turn!");
		activePlayerIndex++;
		if (activePlayerIndex == playerIDs.Count) {
			activePlayerIndex = 0;	
			GrantLocalActionPermission ();
		}
		else {			
			//NetworkInstanceId activeNetID = playerIDs [activePlayerIndex];
			//RpcDoNextTurn (activeNetID);
			Player activePlayer = players[activePlayerIndex];
			activePlayer.RpcGrantActionPermission ();
		}
	}

	private void GrantLocalActionPermission(){
		playerScript.GrantActionPermission ();			
	}
	#endregion
}   
 