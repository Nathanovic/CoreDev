using UnityEngine;
using System.Collections;

[System.Serializable]
public class ChaseState : State {

	private Grid gridScript;

	//chase values:
	[SerializeField]private float chaseSpeedFactor = 1.5f;
	private bool validChase;

	public bool CanChaseTarget(){
		if(baseAI.GetTarget() != null){
			return true;
		}
		return false;
	}

	public void RecalculatePath(){
		Transform target = baseAI.GetTarget ();

		if (target == null) {
			validChase = false;
		}
		else {
			baseAI.RequestPathAsync (target.GetPosition());
		}
	}

	//state implementation:
	public override void Start (){
		validChase = true;
		baseAI.currentSpeedFactor = chaseSpeedFactor;
		baseAI.onReachedTargetNode += RecalculatePath;
		baseAI.RequestPath (baseAI.GetTarget().GetPosition(), true);	
	}
	public override void Run(){
		baseAI.FollowPath ();

		if (!validChase && onState != null) {//determined by 'RecalculatePathToTarget()'
			onState (StateName.patrolling);
		}

		baseAI.CheckPlayerCollision ();
	}
	public override void Complete () {
		baseAI.onReachedTargetNode -= RecalculatePath;
		baseAI.currentSpeedFactor = 1f;
	}
}