using UnityEngine;
using UnityEngine.UI;

//used to display the game info to the players
//this is done with a syncvar
public class GameInfo : MonoBehaviour {

	[SerializeField]private Text activePlayerText;

	private void Awake(){
		TurnManager.instance.onActivePlayerChanged += ChangeActivePlayerText;
	}

	private void ChangeActivePlayerText(int netID){
		//string colorText = ColorUtility.ToHtmlStringRGB (info.playerColor);
		string colorText = "red";
		string playerText = "<color=#" + colorText + ">" + netID + "</color>'s turn!";
		activePlayerText.text = playerText;
	}
}
