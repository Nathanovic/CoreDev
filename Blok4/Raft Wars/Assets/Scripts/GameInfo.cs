using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//used to display the game info to the players
//this is using a rpc call from turnmanager
public class GameInfo : NetworkBehaviour {

	[SerializeField]private Text activePlayerText;
	public int localRaftID;

	public void InitializeLocalPlayer(int raftID){
		localRaftID = raftID;
		Debug.Log("initializing local raft id");
	}

	[ClientRpc]
	public void RpcChangeActivePlayerText(PlayerInfo info){
		string playerText = (info.raftID == localRaftID) ? "Your</color> turn!" : (info.playerName + "</color>'s turn!");
		string colorText = ColorUtility.ToHtmlStringRGB (info.playerColor);
		activePlayerText.text = "<color=#" + colorText + ">" + playerText;
	}
}
