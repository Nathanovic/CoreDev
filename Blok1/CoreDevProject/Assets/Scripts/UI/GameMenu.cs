using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {

	public CanvasGroup pausePanel;
	public CanvasGroup endPanel;

	public Text endingText;

	void Start(){
		pausePanel.alpha = 0f;
		pausePanel.interactable = false;
		pausePanel.blocksRaycasts = false;

		endPanel.alpha = 0f;
		endPanel.interactable = false;
		endPanel.blocksRaycasts = false;

		GameManager.Instance.InitMenuScript (this);
	}

	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)){
			TogglePause ();
		}	
	}

	public void TogglePause(){
		GameManager.Instance.TogglePause ();
		if (GameManager.Instance.IsPaused) {
			pausePanel.alpha = 1f;
			pausePanel.interactable = true;
			pausePanel.blocksRaycasts = true;
		} 
		else {
			pausePanel.alpha = 0f;
			pausePanel.interactable = false;
			pausePanel.blocksRaycasts = false;
		}
	}

	public void ReturnToMenu(){
		GameManager.Instance.ReturnToMenu ();
	}

	public void ShowEndingPanel(bool gameWon, int score){
		endingText.text = gameWon ? "You have won!" : "You lost...";
		endingText.text += "\nScore: " + score.ToString ();

		endPanel.alpha = 1f;
		endPanel.interactable = true;
		endPanel.blocksRaycasts = true;
	}

	public void Restart(){
		Time.timeScale = 1f;
		GameManager.Instance.StartGame ();
	}
}
