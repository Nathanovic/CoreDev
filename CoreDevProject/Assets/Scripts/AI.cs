using UnityEngine;

public class AI : Character {

	protected override void Start(){
		base.Start ();
		//print ("ai start");
	}

	void OnEnable(){
		AIManager.Instance.SubscribeAI (this);
	}
	//void OnDisable geeft een bug, dus als ik een obj pool voor AIs wil gebruiken zal ik iets anders moeten bedenken om te 'desubscriben'

	public bool LookForPlayer(){
		return false;
	}

	//abstract implementation:
	public override void ControlMe(){
		
	}
}