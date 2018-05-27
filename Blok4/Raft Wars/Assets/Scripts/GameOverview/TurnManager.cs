using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//communicates with the players about whether they are allowed to do an action
//the active player can only be updated on the server-side!
//this script however, is active on all clients
public class TurnManager : NetworkBehaviour {

	public static TurnManager instance;
	private GameInfo gameInfo;
	[SerializeField]private GameResult gameEndScript;

	private const float waitTime = 1f;
	private int activePlayerIndex;
	private List<Player> players;

	private int requiredPlayerCount = 2;
	private int playerCount;
	private int deadPlayerCount;

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

			//server always begins (lazyness):
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
		UserStats info = players[0].userInfo;
		Player activePlayer = players [0];
		if (activePlayerIndex == players.Count) {
			activePlayerIndex = 0;	
		}
		else {			
			activePlayer = players[activePlayerIndex];
			info = activePlayer.userInfo;
		}

		if (activePlayer.alive) {
			activePlayer.RpcGrantActionPermission ();
			gameInfo.RpcChangeActivePlayerText (info.userID, info.userName, info.playerColor);
		}
		else {
			ServerNextTurn ();
		}
	}
	#endregion

	public void ServerPlayerDied(Player player){
		deadPlayerCount++;
		if (deadPlayerCount == (players.Count - 1)) {
			Debug.Log("game ended!");

			Player winningPlayer = null;
			string winningPlayerName = "";
			foreach (Player p in players) {
				if (p.alive) {
					winningPlayer = p;
					winningPlayerName = p.userInfo.userName;
				}

				p.userInfo.ServerEndGameReward (p.alive);
			}

			foreach (Player p in players) {
				//string userName = p.userInfo.userName;
				int userScore = p.userInfo.score;
				bool playerWon = p == winningPlayer;
				if (p.isLocalPlayer)
					gameEndScript.ServerShowGameResult (winningPlayerName, userScore, playerWon);
				else
					TargetShowGameResult (p.connectionToClient, winningPlayerName, userScore, playerWon);
			}
		}
	}

	[TargetRpc]
	public void TargetShowGameResult(NetworkConnection target, string winningPlayer, int myScore, bool playerWon){
		gameEndScript.ServerShowGameResult (winningPlayer, myScore, playerWon);
	}
}   
 