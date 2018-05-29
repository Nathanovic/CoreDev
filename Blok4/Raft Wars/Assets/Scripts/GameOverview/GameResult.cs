using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

//used to show the game result and to handle the UI within this panel
public class GameResult : NetworkBehaviour {
	
	private CanvasGroup resultPanel;
	private ScoreHandler scoreScript;

	[SerializeField]private Text playerWinText;
	[SerializeField]private Text errorText;
	[SerializeField]private Text highScoreText;
	[SerializeField]private Text currentScoreText;

	private void Start(){
		scoreScript = transform.parent.GetComponent<ScoreHandler> ();
		resultPanel = GetComponent<CanvasGroup> ();
		resultPanel.DeActivate ();
	}
		
	[TargetRpc]
	public void TargetShowGameResult(NetworkConnection target, string winningPlayer, int myScore, bool playerWon){
		ShowGameResult (winningPlayer, myScore, playerWon);
	}

	public void ShowGameResult(string winningPlayer, int myScore, bool playerWon){
		resultPanel.Activate ();
		playerWinText.text = playerWon ? "You have won!" : winningPlayer + " has won...";
		currentScoreText.text = "my score: " + myScore;
		highScoreText.text = "highscore: loading...";

		scoreScript.InsertScore(ShowHighscore, HandleLoginError, 1, myScore);
	}

	private void ShowHighscore(List<ScoreInfo> info){ 
		if (info.Count == 0) {
			highScoreText.text = "no highscore yet";
			return;
		}

		string userName = info [0].username;
		int score = info [0].score;
		Debug.Log (score);

		highScoreText.text = "highscore (" + userName + "): " + score.ToString();
	}

	private void HandleLoginError(string error){
		errorText.enabled = true;
		Debug.LogWarning (error);
		errorText.text = "couldn't load score";
	}

	public void ReturnToMenu(){
		NetworkManager.Shutdown ();
		SceneManager.LoadScene (0);
	}
}
