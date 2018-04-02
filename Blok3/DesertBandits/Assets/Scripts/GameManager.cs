using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//used for keeping track of the score, showing if the player lost, and restarting
public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[SerializeField]private Text feedbackText;
	[SerializeField]private GameObject restartButton;

	private bool gameEnded;
	private int maxEnemyCount;
	private int currentEnemyCount;

	private void Awake(){
		instance = this;
	}

	private IEnumerator Start(){
		feedbackText.text = "";
		yield return null;
		maxEnemyCount = currentEnemyCount = CombatManager.Instance.potentialTargets.Count - 1;
		UpdateScoreText ();
	}

	public void EnemyDied(){
		if (gameEnded)
			return;
		
		currentEnemyCount--;
		if (currentEnemyCount == 0) {
			feedbackText.text = "You have won!";
			restartButton.SetActive (true);
			gameEnded = true;
			return;
		}

		UpdateScoreText ();
	}

	void UpdateScoreText(){
		string scoreText = currentEnemyCount.ToString () + "/" + maxEnemyCount.ToString () + " left";
		feedbackText.text = scoreText;		
	}

	public void PlayerDied(){
		gameEnded = true;
		restartButton.SetActive (true);
		feedbackText.text = "U died!";
	}

	public void Restart(){
		restartButton.SetActive (true);		
		SceneManager.LoadScene (0);
	}
}
