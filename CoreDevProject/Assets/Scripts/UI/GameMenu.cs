using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {

	public CanvasGroup pausePanel;
	public CanvasGroup victoryPanel;

	void Start(){
		pausePanel.alpha = 0f;
		pausePanel.interactable = false;
		pausePanel.blocksRaycasts = false;
		victoryPanel.alpha = 0f;
		victoryPanel.interactable = false;
		victoryPanel.blocksRaycasts = false;

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

	public void ShowVictoryPanel(){
		victoryPanel.alpha = 1f;
		victoryPanel.interactable = true;
		victoryPanel.blocksRaycasts = true;
	}

	public void Restart(){
		Time.timeScale = 1f;
		GameManager.Instance.StartGame ();
	}
}
