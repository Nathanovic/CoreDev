using UnityEngine;
using System.Collections;
using System;

public class Totem : LevelObject {

	private Faction faction;

	[SerializeField]private Color goodColor;
	[SerializeField]private Color evilColor;

	public bool CanCaputureMe(Faction otherFaction){
		if (!active && faction != otherFaction) {
			return true;
		}
		else {
			return false;
		}
	}

	protected override void Start(){
		base.Start ();
		faction = Faction.Evil;
	}

	public IEnumerator CaptureMe(float captureTime, NoParamEvent callback){
		active = true;
		yield return LoadBar (captureTime);

		if (active) {
			callback ();
			if (faction == Faction.Evil) {
				myRenderer.color = goodColor;
				faction = Faction.Good;
			}
			else {
				myRenderer.color = evilColor;
				faction = Faction.Evil;
			}

			GameManager.Instance.CaptureTotem (faction);
		}

		ResetLoadbarWidth ();
	}
	public void StopCapturing(){
		active = false;
	}
}

public enum Faction{
	Good,
	Evil
}