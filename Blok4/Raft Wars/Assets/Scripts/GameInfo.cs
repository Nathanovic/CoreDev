using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

	[SerializeField]private Text activePlayerText;

	private void Awake(){
		TurnManager.Instance.onActivePlayerChanged += ChangeActivePlayerText;
	}

	private void ChangeActivePlayerText(PlayerInfo info){
		string colorText = ColorUtility.ToHtmlStringRGB (info.playerColor);
		string playerText = "<color=#" + colorText + ">" + info.playerName.ToString() + "</color>'s turn!";
		activePlayerText.text = playerText;
	}
}
