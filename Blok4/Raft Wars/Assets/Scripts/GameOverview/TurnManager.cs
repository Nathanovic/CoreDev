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

	private int requiredPlayerCount = 2;
	private int playerCount;

	[SerializeField]private Color[] colors;

	private void Awake(){		
		instance = this;
		players = new List<Player> ();
		gameInfo = GetComponent<GameInfo> ();

		requiredPlayerCount = GameManager.instance.GetPlayerCount ();
	}

	#region connection handling
	//only called on the server
	public void InitializeServerPlayer(Player player){ 
		//Debug.Log ("initialize player: " + player.connectionToClient + "_" + player.connectionToServer);

		players.Add (player);
		//player.userInfo.SetUserData (userID, userName, colors [playerCount]);

		if (!player.isLocalPlayer)
			TargetInitializeLocalGameInfo (player.connectionToClient, playerCount);
		else
			InitializeGameInfo (playerCount);

		playerCount++;

		if (playerCount == requiredPlayerCount) {
			for (int i = 0; i < players.Count; i ++) {
				players [i].RpcInitializeRaft (i, colors[i]);
				players [i].userInfo.ServerSetUserColor (colors [i]);
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
		User info = players[0].userInfo;
		Player activePlayer = players [0];
		if (activePlayerIndex == players.Count) {
			activePlayerIndex = 0;	
		}
		else {			
			activePlayer = players[activePlayerIndex];
			info = activePlayer.userInfo;
		}

		activePlayer.RpcGrantActionPermission ();
		gameInfo.RpcChangeActivePlayerText (info.userID, info.userName, info.playerColor);
	}
	#endregion
}   
 