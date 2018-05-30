using UnityEngine;
using UnityEngine.UI;

public class PlayerCount : MonoBehaviour {

	private Text countText;

	private void Start () {
		countText = GetComponent<Text> ();
		UpdatePlayerText (2f);
	}
	
	public void UpdatePlayerText(float count){
		countText.text = Mathf.RoundToInt (count) + " players";
	}
}
