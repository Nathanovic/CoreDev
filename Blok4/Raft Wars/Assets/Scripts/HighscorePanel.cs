using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//showing the highscores in the menu scene
public class HighscorePanel : MonoBehaviour {

	private ScoreHandler scoreScript;
	[SerializeField]private NetworkHUD networkPanelScript;
	private string condition = "";

	private CanvasGroup panel;
	[SerializeField]private UserScoreTextRow[] textRows = new UserScoreTextRow[3];
	[SerializeField]private Text waitText;

	private void Start () {
		scoreScript = transform.parent.GetComponent<ScoreHandler> ();
		panel = GetComponent<CanvasGroup> ();
		panel.DeActivate ();
	}

	public void ToggleTodayOnly(bool isTrue){
		condition = isTrue ? "today" : "";
		Show ();
	}
	
	public void Show(){
		panel.Activate ();
		waitText.enabled = true;

		scoreScript.GetScores (GetScores, null, textRows.Length, condition);
	}

	private void GetScores(List<ScoreInfo> scoreInfo){
		waitText.enabled = false;
		for (int i = 0; i < textRows.Length; i ++) {
			UserScoreTextRow row = textRows [i];
			if (i < scoreInfo.Count) {
				ScoreInfo data = scoreInfo [i];
				row.userNameText.text = data.username;
				row.userScoreText.text = data.score.ToString ();
			}
			else {
				row.userNameText.text = "-";
				row.userScoreText.text = "-";
			}
		}
	}

	public void ReturnToMenu(){
		panel.DeActivate ();
		networkPanelScript.EnableGameHUD ();
	}

	[System.Serializable]
	public class UserScoreTextRow{
		public Text userNameText;
		public Text userScoreText;
	}
}
