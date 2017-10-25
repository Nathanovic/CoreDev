using UnityEngine;
using System.Collections;

public class GoodTrap : LevelObject {

	[SerializeField]private Color activeColor;
	[SerializeField]private Color deactivatedColor;

	[SerializeField]private float placeLoadTime = 1f;
	[SerializeField]private float restoreTime = 8f;

	private AI trappedTarget;

	public void PlaceMe(Vector2 targetPos){
		transform.SetPosition (targetPos);
		DeactivateSelf ();
		StartCoroutine (ReactivateSelf (placeLoadTime));
	}

	public void Triggered(AI other, float trapTime){
		trappedTarget = other;
		myRenderer.color = deactivatedColor;
		StartCoroutine (TriggerSelf (trapTime));
	}

	private IEnumerator TriggerSelf(float trapTime){
		yield return LoadBar(trapTime);

		if (active) {
			trappedTarget.onConverted ();
		}

		DeactivateSelf ();
		StartCoroutine (ReactivateSelf(restoreTime));
	}

	private void DeactivateSelf(){
		myRenderer.color = deactivatedColor;
		active = false;
	}

	private IEnumerator ReactivateSelf(float activationTime){
		while (loadPercentage > 0f) {
			ModifyLoadbarWidth(-Time.deltaTime / activationTime);
			yield return null;
		}

		myRenderer.color = activeColor;
		active = true;
	}
}
