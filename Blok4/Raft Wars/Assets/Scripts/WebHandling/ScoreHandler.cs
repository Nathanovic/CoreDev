using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour {

	private const string HIGHSCORE_URL = "studenthome.hku.nl/~nathan.flier/RaftWars_DB/highscores.php";
	private const string SCORE_KEY = "score";
	private const string USER_KEY = "name";
	private const string CONDITION_KEY = "condition";

	private UnityAction<List<ScoreInfo>> returnScoresCallback;
	private UnityAction<string> returnErrorCallback;

	[SerializeField]private Text errorText;
	
	public void GetScores(UnityAction<List<ScoreInfo>> callback, UnityAction<string> errorCallback, int scoreCount, string condition = ""){
		returnScoresCallback = callback;
		returnErrorCallback = errorCallback;

		StartCoroutine(WebHandler.instance.ProcessWebRequest (HIGHSCORE_URL, GetScores, HandleScoreError, 
			"limit", scoreCount.ToString(), CONDITION_KEY, condition));
	}

	private void GetScores(string result){
		JArray jArray = JArray.Parse (result);
		int scoreCount = jArray.Count;
		List<ScoreInfo> scoreDict = new List<ScoreInfo> (scoreCount);

		for (int i = 0; i < scoreCount; i++) {
			JToken jsonObject = jArray [i];
			string userName = jsonObject [USER_KEY].ToObject<string> ();
			int score = jsonObject [SCORE_KEY].ToObject<int> ();
			scoreDict.Add (new ScoreInfo(userName, score));
		}

		returnScoresCallback (scoreDict);

		ResetCallbacks ();
	}

	public void InsertScore(UnityAction<List<ScoreInfo>> callback, UnityAction<string> errorCallback, int scoreCount, int achievedScore){
		returnScoresCallback = callback;
		returnErrorCallback = errorCallback;

		StartCoroutine(WebHandler.instance.ProcessWebRequest (HIGHSCORE_URL, GetScores, HandleScoreError, 
			"limit", scoreCount.ToString(), "achievedScore", achievedScore.ToString()));
	}

	private void HandleScoreError(string error){
		errorText.enabled = true;
		errorText.text = error;

		if (returnErrorCallback != null) {
			returnErrorCallback (error);

			ResetCallbacks ();
		}
	}

	private void ResetCallbacks(){
		returnErrorCallback = null;
		returnErrorCallback = null;		
	}
}

public struct ScoreInfo{
	public string username;
	public int score;

	public ScoreInfo(string _name, int _score){
		username = _name;
		score = _score;
	}
}
