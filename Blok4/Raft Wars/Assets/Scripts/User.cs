using UnityEngine;
using UnityEngine.Networking;

//keeps all of the userdata (on the server!)
//if the player is connected, he passes it's data to the server with a command 
public class User : NetworkBehaviour{

	//this data is used to let the player know who has won
	public int userID;
	public string userName;
	public Color playerColor;

	public void Login(int _id, string _name){
		userID = _id;
		userName = _name;
	}

	public void RegisterSelf (int _id, string _name) {
		if (isServer)
			SetUserData (_id, _name);
		else
			CmdSetUserData (_id, _name);
	}

	[Command]
	private void CmdSetUserData(int id, string name){
		SetUserData(id, name);
	}

	private void SetUserData(int id, string name){
		userID = id;
		userName = name;
	}

	/*
	public void SetUserData(int id, string name, Color color){
		userID = id;
		userName = name;
		playerColor = color;
	}
	*/

	public void ServerSetUserColor(Color c){
		playerColor = c;
	}
}
