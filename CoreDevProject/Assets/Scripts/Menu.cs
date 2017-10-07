using UnityEngine;

public class Menu : MonoBehaviour {

	public void StartGame(){
		GameManager.Instance.StartGame ();
	}

	public void QuitGame(){
		GameManager.Instance.QuitGame ();
	}
}
