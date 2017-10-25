using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//doel:
//ik kan ai's maken die hiervan inheriten en waar ik gedrag in kan stoppen door ze een list met states te geven
//basis gedrag:
//-patrolling
//-chasing target
//verschillend gedrag:
//-capture totem
//-trapped
//-special move

//this AI can be either good or evil (dependant on targetingBehaviour)
//targetingbehaviour is completely managed inside this script!
//is patrolling until a better target is found
//this 'better target' can be found by TargetingBehaviour
//movement is completely handled in this script
public class AI : Character {

	protected FSM fsm;//FSM
	[SerializeField]private PatrolState patrolState = new PatrolState();
	public ChaseState chaseState = new ChaseState();

	[SerializeField]private float trapTime = 1f;

	[SerializeField]private bool displayPathGizmos;

	private PathState pathState;
	public PathState CurrentPathState {
		get { 
			return pathState;
		}
		private set { 
			pathState = value;
		}
	}
	private Vector2[] path;
	private int targetIndex = 0;
	private Vector2 CurrentWaypoint{
		get{ 
			return path[targetIndex];
		}
	}
	private bool asyncRequestRunning;

	private TargetingBehaviour targetingBehaviour;//Strategy pattern
	public Transform GetTarget(){
		return targetingBehaviour.GetTarget ();
	}

	public event NoParamEvent onConverted;

	protected override void Start(){
		myFaction = Faction.Evil;
		CurrentPathState = PathState.none;
		AIManager.Instance.SubscribeAI (this, out targetingBehaviour);

		base.Start ();

		fsm = new FSM (this);
		fsm.AddState (StateName.patrolling, patrolState);
		fsm.AddState (StateName.chasing, chaseState);
		fsm.SetState (StateName.patrolling);

		onConverted += ConvertSelf;
	}

	public void UpdateFSM(){
		fsm.Update ();
	}

	//path behaviour (for chasing target & for patrolling)
	public void RequestPathAsync(Vector2 targetPos){
		if(!asyncRequestRunning && CurrentPathState != PathState.requested){
			asyncRequestRunning = true;
			RequestPath (targetPos, true);
		}
	}
	public void RequestPath(Vector2 targetPos, bool forcedRequest = false){
		if (CurrentPathState == PathState.none || forcedRequest) {
			CurrentPathState = PathState.requested;
			PathRequestManager.RequestPath (transform.GetPosition (), targetPos, OnPathFound);
		}
	}
	private void OnPathFound(Vector2[] newPath, bool pathSuccesful){
		if (pathSuccesful) {
			path = newPath;
			CurrentPathState = PathState.following;
			targetIndex = 0;
		}
		else {
			CurrentPathState = PathState.none;
		}

		asyncRequestRunning = false;
	}

	public void FollowPath(){
		if (CurrentPathState == PathState.following) {
			MoveToPosition (CurrentWaypoint);
			if (transform.GetPosition () == CurrentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					CurrentPathState = PathState.none;
				}
			}
		}
	}

	private void OnDrawGizmos(){
		Gizmos.color = Color.black;
		if (CurrentPathState == PathState.following && path != null && targetIndex < path.Length) {
			Gizmos.DrawLine (transform.position, CurrentWaypoint);
			Gizmos.DrawSphere (CurrentWaypoint, 0.1f);
			for (int i = targetIndex + 1; i < path.Length; i++) {
				Gizmos.DrawLine (path[i - 1], path [i]);
				Gizmos.DrawSphere (path [i], 0.1f);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "ForgiveTrap" && myFaction == Faction.Evil) {
			GoodTrap trap = other.GetComponent<GoodTrap> ();
			onReachedTargetNode += ReachedTrap;
		}
	}

	private void ReachedTrap(){
		onReachedTargetNode -= ReachedTrap;
		print ("I wanne trap self");
	}

	private void ConvertSelf(){
		myFaction = Faction.Good;
		targetingBehaviour.UpdateTargetingBehaviour (new FriendlyTargeting ());
	}
}

public enum PathState{
	following,
	requested,
	none
}