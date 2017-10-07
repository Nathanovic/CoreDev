using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {

	public CanvasGroup menuCVG;

	void Start(){
		menuCVG.alpha = 0f;
	}

	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)){
			TogglePause ();
		}	
	}

	public void TogglePause(){
		GameManager.Instance.TogglePause ();
		if (GameManager.Instance.IsPaused) {
			menuCVG.alpha = 1f;
		} 
		else {
			menuCVG.alpha = 0f;
		}
	}

	public void ReturnToMenu(){
		GameManager.Instance.ReturnToMenu ();
	}
}
