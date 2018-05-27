using UnityEngine;
using UnityEngine.Networking;

//local userID and userName is set on Login
//all of the userdata is stored on the serverside
//if the player is connected, he passes it's data to the server with a command 
public class UserStats : NetworkBehaviour{

	//local & server data:
	public int userID;
	public string userName;

	//server data:
	public Color playerColor;
	public int score;//hitted -1, hit other + 1, lose -1, win + 3

	private void Start(){
		if (NetworkManager.singleton.isNetworkActive && isServer) {
			GetComponent<Health> ().onServerHealthChanged += ServerOnDamageTaken;
			GetComponent<RaftCombat> ().onServerProjectileHit += ServerOnDamageDealt;
		}
	}

	public void Login(int _id, string _name){
		userID = _id;
		userName = _name;
	}

	public void RegisterSelf (int _id, string _name) {
		CmdSetUserData (_id, _name);
	}

	[Command]
	private void CmdSetUserData(int id, string name){
		userID = id;
		userName = name;
	}

	public void ServerSetUserColor(Color c){
		playerColor = c;
	}

	private void ServerOnDamageTaken(int dmg){
		score -= dmg;
	}

	private void ServerOnDamageDealt(int dmg){
		score += dmg;
	}

	public void ServerEndGameReward(bool playerWon) {
		score += playerWon ? 3 : -1;
	}
}
