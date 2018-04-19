using System.Collections.Generic;
using UnityEngine;

//communicates with the players about whether they are allowed to do an action
//this script is only ran on the server!
public class TurnManager : MonoBehaviour {

	private static TurnManager _instance;
	public static TurnManager Instance{
		get{ 
			if (_instance == null) {
				GameObject obj = new GameObject ("Turn Manager");
				obj.AddComponent<TurnManager> ();
			}

			return _instance;
		}
	}

	private const float waitTime = 1f;
	private List<Player> players;
	private int activePlayerIndex;

	public delegate void PlayerDelegate(PlayerInfo playerInfo);
	public event PlayerDelegate onActivePlayerChanged;

	private void Awake(){
		_instance = this;
		players = new List<Player> ();
	}

	public void InitPlayer(Player player){
		players.Add (player);
	}

	private void Start () {
		//choose first player random
		int rndmPlayerIndex = Random.Range (0, players.Count);
		activePlayerIndex = rndmPlayerIndex;
		NextTurn ();
	}

	public void NextTurn(){
		activePlayerIndex++;
		if (activePlayerIndex == players.Count) {
			activePlayerIndex = 0;
		}

		Player activePlayer = players [activePlayerIndex];
		activePlayer.GrantActionPermission ();
		if (onActivePlayerChanged != null) {
			onActivePlayerChanged (activePlayer.myInfo);
		}
	}
}
