using UnityEngine;

//used to move data from the menu to the game
public class GameManager : MonoBehaviour {

	public static GameManager instance;

	private int playerCount = 2;

	public bool loggedIn{ get; private set; }
	public int userID{ get; private set; }
	public string userName{ get; private set; }
	public string password{ get; private set; }

	private void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else {
			Destroy (gameObject);
		}
	}

	private void Start(){
		SetPlayerCount (playerCount);
	}

	public void SetPlayerCount(float newPlayerCount){
		playerCount = Mathf.RoundToInt(newPlayerCount);
	}

	public int GetPlayerCount(){
		return playerCount;
	}

	public void PlayerLoggedIn(string _userName, int _userID, string _password){
		loggedIn = true;
		userName = _userName;
		userID = _userID;
		password = _password;
	}
}
