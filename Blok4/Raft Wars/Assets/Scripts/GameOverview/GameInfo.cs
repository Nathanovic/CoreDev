using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//used to display the game info to the players
//this is using a rpc call from turnmanager
public class GameInfo : NetworkBehaviour {

	[SerializeField]private Text activePlayerText;
	public int localUserID;

	public void InitializeLocalPlayer(int userID){
		localUserID = userID;
	}

	[ClientRpc]
	public void RpcChangeActivePlayerText(int userID, string userName, Color userColor){
		string playerText = (localUserID == userID) ? "Your</color> turn!" : (userName + "</color>'s turn!");
		string colorText = ColorUtility.ToHtmlStringRGB (userColor);
		activePlayerText.text = "<color=#" + colorText + ">" + playerText;
	}
}
