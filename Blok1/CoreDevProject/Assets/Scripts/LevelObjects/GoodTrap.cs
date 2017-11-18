using UnityEngine;
using System.Collections;

public class GoodTrap : LevelObject {

	[SerializeField]private Color activeColor;
	[SerializeField]private Color deactivatedColor;

	[SerializeField]private float placeLoadTime = 1f;
	[SerializeField]private float restoreTime = 8f;

	private AI trappedTarget;
	public TrapState myState;

	public bool CanTriggerMe{
		get{ 
			if (myState == TrapState.active) {
				return true;
			}
			return false;
		}
	}

	protected override void Start(){
		base.Start ();
		PlaceMe(transform.GetPosition());
	}

	public void PlaceMe(Vector2 targetPos){
		gameObject.SetActive (true);
		transform.SetPosition (targetPos);

		DeactivateSelf ();
		StartCoroutine (ReactivateSelf (placeLoadTime));
	}

	public void Dismantle(){
		myState = TrapState.dismantled;
		gameObject.SetActive (false);
	}

	public void Triggered(AI other, float trapTime){
		trappedTarget = other;
		myRenderer.color = deactivatedColor;
		StartCoroutine (TriggerSelf (trapTime));
	}

	private IEnumerator TriggerSelf(float trapTime){
		myState = TrapState.loading;
		yield return LoadBar(trapTime);

		if (active) {
			trappedTarget.ChangeStrategy ();
		}

		DeactivateSelf ();
		StartCoroutine (ReactivateSelf(restoreTime));
	}

	private void DeactivateSelf(){
		myState = TrapState.loading;
		myRenderer.color = deactivatedColor;
		trappedTarget = null;
		active = false;
	}

	private IEnumerator ReactivateSelf(float activationTime){
		loadPercentage = 1f;
		while (loadPercentage > 0f) {
			ModifyLoadbarWidth(-Time.deltaTime / activationTime);
			yield return null;
		}

		myState = TrapState.active;
		myRenderer.color = activeColor;
		active = true;
	}
}

[System.Serializable]
public enum TrapState{
	loading,
	active,
	dismantled
}