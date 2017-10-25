using UnityEngine;
using System.Collections;
using System;

//deze class houdt bij of de AI goed is of slecht en bepaald op basis hiervan wat 
//er gebeurd on collision, en wat de target van deze AI kan worden
public class TargetingBehaviour{

	private TargetStrategy currentBehaviour;

	public TargetingBehaviour(TargetStrategy myStrategy){
		currentBehaviour = myStrategy;
	}

	public void UpdateTargetingBehaviour(TargetStrategy newStrategy){
		currentBehaviour = newStrategy;
	}

	public Transform GetTarget(){
		return currentBehaviour.GetTarget ();
	}
}

public abstract class TargetStrategy{

	public abstract Transform GetTarget ();
}

//probeer speler(of goede AI's) op te eten
public class EvilTargeting : TargetStrategy{

	private Transform player;

	public EvilTargeting(Transform _player){
		player = _player;
	}

	public override Transform GetTarget () {
		return player;
	}
}

//probeer totems mooi te maken
public class FriendlyTargeting : TargetStrategy{

	public override Transform GetTarget () {
		return UnityEngine.Object.FindObjectOfType<Transform> ();
	}
}