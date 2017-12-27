using UnityEngine;
using System.Collections;

public class Bridge : Building {

	public void ColorMe(){
		GetComponent<SpriteRenderer> ().color = Random.ColorHSV ();
	}
	public void ResetMe(){
		GetComponent<SpriteRenderer> ().color = Color.white;
	}

}