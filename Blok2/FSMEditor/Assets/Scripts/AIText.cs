using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIText : MonoBehaviour {

	private Text myText;
	public Transform target;
	private AIEvents eventScript;
	public float yOffset = 0.8f;

	void Start () {
		myText = GetComponent<Text> ();
		eventScript = target.GetComponent<AIEvents> ();
		eventScript.onTargetDies += KilledEnemyFeedback;
		eventScript.onStartPatrol += ShowPatrolTexts;
	}

	void Update(){
		Vector3 myPos = target.position;
		myPos.y += yOffset;
		transform.position = myPos;
	}

	void ShowPatrolTexts(){
		StartCoroutine (ShowTexts (1.5f, 0.5f, "Pom", "Pompiedom", "Dadiedom"));
		eventScript.onStartPatrol -= ShowPatrolTexts;
	}
	
	void KilledEnemyFeedback(){
		StartCoroutine(ShowTexts(0.8f, 0.3f, "Haha!", "I'm stronger!"));
	}

	IEnumerator ShowTexts(float timeToShow, float noShowTime, params string[] texts){
		for (int i = 0; i < texts.Length; i++) {
			myText.text = texts [i];
			yield return new WaitForSeconds (timeToShow);
			myText.text = "";
			yield return new WaitForSeconds (noShowTime);
		}
	}
}
