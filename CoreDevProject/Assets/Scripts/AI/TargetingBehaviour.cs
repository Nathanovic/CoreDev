using UnityEngine;
using System.Collections;
using System;

//deze class houdt bij of de AI goed is of slecht en bepaald op basis hiervan wat 
//er gebeurd on collision, en wat de target van deze AI kan worden
public class TargetingBehaviour{

	private TargetStrategy currentBehaviour;

	public TargetingBehaviour(TargetStrategy myStrategy){
		UpdateTargetingBehaviour (myStrategy);
	}

	public void UpdateTargetingBehaviour(TargetStrategy newStrategy){
		currentBehaviour = newStrategy;
		currentBehaviour.ActivateMe ();
	}

	public Transform GetTarget(){
		return currentBehaviour.GetTarget ();
	}

	public Character GetPlayerCollision(){
		return currentBehaviour.PlayerCollision ();
	}
}

public abstract class TargetStrategy{

	protected Transform transform;
	private SpriteRenderer myRenderer;
	[SerializeField]private Sprite mySprite;

	[SerializeField]private float maxDistToTarget = 5f;
	[SerializeField]protected float radius = 0.49f;
	protected float sqrMaxDistToTarget;

	protected void Init(Transform _transform){
		transform = _transform;
		myRenderer = _transform.GetComponent<SpriteRenderer> ();
		sqrMaxDistToTarget = maxDistToTarget * maxDistToTarget;
	}

	public void ActivateMe(){//not yet
		myRenderer.sprite = mySprite;
	}

	protected float SqrDist(Vector2 otherPos){
		Vector2 offset = otherPos - transform.GetPosition();
		return offset.sqrMagnitude;
	}

	public abstract Transform GetTarget ();

	public abstract Character PlayerCollision ();
}

//probeert speler(of goede AI's) op te eten
[System.Serializable]
public class EvilTargeting : TargetStrategy{

	[SerializeField]private float stickyness = 1.5f;
	private float sqrtStickyness;

	private Transform player;
	private LayerMask targetLM;

	public void Init(Transform _transform, Transform _player){
		base.Init (_transform);
		player = _player;
		sqrtStickyness = stickyness * stickyness;
		targetLM = LayerMask.GetMask ("Player", "AI");
	}

	public override Transform GetTarget () {
		if (SqrDist (player.GetPosition ()) < (sqrMaxDistToTarget + sqrtStickyness)) {
			return player;
		}

		return null;
	}

	public override Character PlayerCollision () {
		Collider2D[] playerColls = Physics2D.OverlapCircleAll (transform.GetPosition (), radius, targetLM);
		foreach(Collider2D c in playerColls){
			Character characterScript = c.GetComponent<Character> ();
			if (characterScript.myFaction == Faction.Good) {
				return c.GetComponent<Character> ();
			}
		}

		return null;
	}
}

//probeert totems over te nemen voor de speler
[System.Serializable]
public class FriendlyTargeting : TargetStrategy{

	public Transform[] patrolPoints;//worden 3 verschillende dingen die eenmalig doorgegeven worden aan patrolState
	private Transform[] targets;//worden de totemTransforms als deze AI een Capture component heeft
	private Totem[] totems;
	private bool canCapture;

	new public void Init(Transform _transform){
		base.Init (_transform);

		if (transform.GetComponent<Capture> () == null) {
			return;
		}		

		canCapture = true;

		totems = UnityEngine.Object.FindObjectsOfType<Totem> ();
		targets = new Transform[totems.Length];
		for(int i = 0; i < targets.Length; i ++){
			targets [i] = totems [i].transform;
		}
	}

	public override Transform GetTarget () {
		float closestSqrDist = sqrMaxDistToTarget;
		Transform target = null;

		if (canCapture && totems.Length > 0) {
			for (int i = 0; i < totems.Length; i++) {
				Transform targetI = targets [i];
				if (totems [i].CanCaputureMe (Faction.Good) && transform.GetPosition () != targetI.GetPosition ()) {
					float sqrDist = SqrDist (targetI.GetPosition ());
					if (sqrDist < closestSqrDist) {
						target = targetI;
					}
				}
			}
		}

		return target;
	}

	public override Character PlayerCollision () {
		return null;
	}
}