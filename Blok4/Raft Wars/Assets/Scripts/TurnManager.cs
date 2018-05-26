using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//communicates with the players about whether they are allowed to do an action
//the active player can only be updated on the server-side!
//this script however, is active on all clients
public class TurnManager : NetworkBehaviour {

	public static TurnManager instance;
	private GameInfo gameInfo;

	private const float waitTime = 1f;
	private int activePlayerIndex;
	private List<Player> players;
	public List<NetworkConnection> connections;

	private int requiredPlayerCount = 2;
	private int playerCount;

	private void Awake(){		
		instance = this;
		players = new List<Player> ();
		connections = new List<NetworkConnection> ();
		gameInfo = GetComponent<GameInfo> ();

		requiredPlayerCount = GameManager.instance.GetPlayerCount ();
	}

	#region connection handling
	//only called on the server
	public void InitializeServerPlayer(Player player){ 
		Debug.Log ("initialize player: " + player.connectionToClient + "_" + player.connectionToServer);
		connections.Add (player.connectionToClient);

		players.Add (player);
		player.myInfo.raftID = playerCount;
		player.myInfo.netID = player.netId;

		if (!player.isLocalPlayer)
			TargetInitializeLocalGameInfo (player.connectionToClient, playerCount);
		else
			InitializeGameInfo (playerCount);

		playerCount++;

		if (playerCount == requiredPlayerCount) {
			foreach (Player p in players) {
				p.RpcInitializeRaft (p.myInfo.raftID);
			}

			//server always begins:
			activePlayerIndex = players.Count - 1;
			ServerNextTurn ();
		}
	}

	[TargetRpc]
	private void TargetInitializeLocalGameInfo(NetworkConnection target, int raftID){
		InitializeGameInfo (raftID);
	}

	private void InitializeGameInfo(int raftID){
		gameInfo.InitializeLocalPlayer (raftID);
	}
	#endregion

	#region turn handling
	public void ServerNextTurn(){
		activePlayerIndex++;
		PlayerInfo playerInfo = players[0].myInfo;
		Player activePlayer = players [0];
		if (activePlayerIndex == players.Count) {
			activePlayerIndex = 0;	
		}
		else {			
			activePlayer = players[activePlayerIndex];
			playerInfo = activePlayer.myInfo;
		}

		activePlayer.RpcGrantActionPermission ();
		gameInfo.RpcChangeActivePlayerText (playerInfo);
	}
	#endregion
}   
 