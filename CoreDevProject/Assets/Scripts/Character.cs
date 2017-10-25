using UnityEngine;
using System.Collections;

public delegate void CharacterEvent(Character sender);
public delegate void NoParamEvent();

//abstract class for ai & player
public abstract class Character : MonoBehaviour {

	protected Grid grid;
	[SerializeField]protected float moveSpeed = 1f;
	[HideInInspector]public float currentSpeedFactor = 1f;
	public Faction myFaction{ get; protected set; }

	public event CharacterEvent onDeath;
	public event NoParamEvent onReachedTargetNode;

	protected virtual void Start(){
		grid = FindObjectOfType<Grid> ();
		transform.SetPosition(grid.NodePosFromWorldPoint(transform.GetPosition()));
	}
		
	//other methods:
	protected void MoveToPosition (Vector2 targetPos){
		float deltaMovement = moveSpeed * Time.deltaTime * currentSpeedFactor;
		Vector2 newPos = Vector2.MoveTowards (transform.GetPosition(), targetPos, deltaMovement);
		transform.SetPosition (newPos);

		if (targetPos == newPos && onReachedTargetNode != null) {
			onReachedTargetNode ();
		}
	}
}
