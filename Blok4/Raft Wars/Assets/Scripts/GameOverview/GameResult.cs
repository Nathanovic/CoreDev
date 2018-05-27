using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//used to show the game result and to handle the UI within this panel
public class GameResult : NetworkBehaviour {

	private CanvasGroup resultPanel;

	[SerializeField]private Text playerWinText;
	[SerializeField]private Text errorText;
	[SerializeField]private Text highScoreText;
	[SerializeField]private Text currentScoreText;

	private void Start(){
		resultPanel = GetComponent<CanvasGroup> ();
		resultPanel.DeActivate ();
	}
		
	[TargetRpc]
	public void TargetShowGameResult(NetworkConnection target, string winningPlayer, int myScore, bool playerWon){
		ServerShowGameResult (winningPlayer, myScore, playerWon);
	}

	public void ServerShowGameResult(string winningPlayer, int myScore, bool playerWon){
		resultPanel.Activate ();
		playerWinText.text = playerWon ? "You have won!" : winningPlayer + " has won...";
		currentScoreText.text = "my score: " + myScore;
	}

	public void SubmitScore(){
		
	}

	public void ReturnToMenu(){
		
	}
}
