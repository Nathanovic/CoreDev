using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//used to display the game info to the players
//this is done with a syncvar
public class GameInfo : MonoBehaviour {

	[SerializeField]private Text activePlayerText;

	private void Awake(){
		//TurnManager.instance.onActivePlayerChanged += ChangeActivePlayerText;
	}

	private void ChangeActivePlayerText(PlayerInfo info){
		string colorText = ColorUtility.ToHtmlStringRGB (info.playerColor);
		string playerText = "<color=#" + colorText + ">" + info.playerName + "</color>'s turn!";
		activePlayerText.text = playerText;
	}
}
