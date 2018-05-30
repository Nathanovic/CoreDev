using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//custom network HUD
public class NetworkHUD : MonoBehaviour {

	private CanvasGroup connectionPanel;

	[SerializeField]private HighscorePanel highscoreScript;
	[SerializeField]private Text connText;
	[SerializeField]private Text userText;

	private void Start(){
		connectionPanel = GetComponent<CanvasGroup> ();
		connectionPanel.DeActivate ();
	}

	public void EnableGameHUD(string userName){
		connectionPanel.Activate ();
		userText.text = "Welcome " + userName + "!";
	}
	public void EnableGameHUD(){
		connectionPanel.Activate ();
	}

	public void StartHost(){
		NetworkManager.singleton.StartHost();
		connText.text = "Setting up host...";
	}

	public void StartClientHome(){
		NetworkManager.singleton.StartClient ();	
		connText.text = "Connecting to server...";
	}

	public void ShowHighscores(){
		connectionPanel.DeActivate ();
		highscoreScript.Show ();
	}
}

public static class ExtensionMethods{

	public static void Activate(this CanvasGroup panel){
		panel.alpha = 1f;
		panel.interactable = true;
		panel.blocksRaycasts = true;
	}

	public static void DeActivate(this CanvasGroup panel){
		panel.alpha = 0f;
		panel.interactable = false;
		panel.blocksRaycasts = false;
	}
}