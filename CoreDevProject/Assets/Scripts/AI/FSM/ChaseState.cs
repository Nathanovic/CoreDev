using UnityEngine;
using System.Collections;

[System.Serializable]
public class ChaseState : State {

	private Transform transform;
	private Grid gridScript;

	[SerializeField]private float maxDistToTarget;
	[SerializeField]private float stickyness = 1.5f;

	//chase values:
	[SerializeField]private float chaseSpeedFactor = 1.5f;
	private Vector2 targetPos;
	private bool validChase;

	public void SetTargetPos(){
		Transform target = baseAI.GetTarget ();
		targetPos = target.GetPosition ();
	}
	public Vector2 CurrentTargetPos(){
		Transform target = baseAI.GetTarget ();
		return target.GetPosition ();
	}

	public override void Init (AI _target){
		base.Init (_target);
		transform = _target.transform;
	}

	public bool CanChaseTarget(){
		if(Vector2.Distance(transform.GetPosition(), CurrentTargetPos()) < maxDistToTarget){
			return true;
		}
		return false;
	}

	public void RecalculatePathToTarget(){
		Vector2 newPos = CurrentTargetPos ();

		if(Vector2.Distance(newPos, targetPos) > stickyness && baseAI.CurrentPathState != PathState.requested){
			SetTargetPos ();
			if (Vector2.Distance (transform.GetPosition (), newPos) > (maxDistToTarget + stickyness)) {
				validChase = false;
			}
			else {
				//update path:
				baseAI.RequestPathAsync (newPos);
			}
		}
	}
	public void CheckCollisionWithPlayer(){
	
	}

	//state implementation:
	public override void Start (){
		SetTargetPos();
		validChase = true;
		baseAI.currentSpeedFactor = chaseSpeedFactor;
		baseAI.onReachedTargetNode += RecalculatePathToTarget;
		baseAI.RequestPath (targetPos, true);	
	}
	public override void Run(){
		baseAI.FollowPath ();

		if (!validChase) {//determined by 'RecalculatePathToTarget()'
			onState (StateName.patrolling);
		}
		else if (baseAI.CurrentPathState == PathState.none) {
			baseAI.RequestPath (CurrentTargetPos (), true);
		}

		CheckCollisionWithPlayer ();
	}
	public override void Complete () {
		baseAI.onReachedTargetNode -= RecalculatePathToTarget;
		baseAI.currentSpeedFactor = 1f;
	}
}