using UnityEngine;
using UnityEngine.UI;

//only active on the server???, keeps track of the game progression and score
public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public bool loggedIn{ get; private set; }
	public string userName{ get; private set; }
	public Text playerCountText;
	private int playerCount = 2;
	private string nickname = "player";

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
		playerCountText.text = playerCount + " players";
	}

	public int GetPlayerCount(){
		return playerCount;
	}

	public void SetNickname(string newName){
		nickname = newName;
	}

	public string GetNickname(){
		return nickname;
	}

	public void PlayerLoggedIn(string _userName){
		loggedIn = true;
		userName = _userName;
	}
}
