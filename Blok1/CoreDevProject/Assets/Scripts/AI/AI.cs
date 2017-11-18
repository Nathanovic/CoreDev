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
//targetingbehaviour is completely managed inside the chaseState!
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
	private Vector2 currentWaypoint;
	private bool asyncRequestRunning;

	private TargetingBehaviour targetingBehaviour;//Strategy pattern
	public Transform GetTarget(){
		return targetingBehaviour.GetTarget ();
	}
		
	[SerializeField]private EvilTargeting evilTargeting = new EvilTargeting(); 
	[SerializeField]private FriendlyTargeting friendlyTargeting = new FriendlyTargeting(); 

	public event NoParamEvent onTrapActivated;
	private GoodTrap trap;

	protected override void Start(){
		myFaction = Faction.Evil;
		CurrentPathState = PathState.none;

		Transform playerTarget = null;
		AIManager.Instance.SubscribeAI (this, out playerTarget);

		evilTargeting.Init (transform, playerTarget);
		friendlyTargeting.Init (transform);
		targetingBehaviour = new TargetingBehaviour (evilTargeting);

		base.Start ();

		fsm = new FSM (this);
		fsm.AddState (StateName.patrolling, patrolState);
		fsm.AddState (StateName.chasing, chaseState);
		fsm.AddState (StateName.trapped, new TrappedState());
		fsm.SetState (StateName.patrolling);
	}

	public void UpdateFSM(){
		fsm.Update ();
	}

	public void RemoveState(StateName stateKey){
		fsm.RemoveState (stateKey);
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
			currentWaypoint = path [0];
		}
		else {
			CurrentPathState = PathState.none;
		}

		asyncRequestRunning = false;
	}

	public void FollowPath(){
		if (CurrentPathState == PathState.following) {
			MoveToPosition (currentWaypoint);
			if (transform.GetPosition () == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					CurrentPathState = PathState.none;
				}
				else {
					currentWaypoint = path [targetIndex];
				}
			}
		}
	}

	private void OnDrawGizmos(){
		Gizmos.color = Color.black;
		if (CurrentPathState == PathState.following && path != null && targetIndex < path.Length) {
			Gizmos.DrawLine (transform.position, currentWaypoint);
			Gizmos.DrawSphere (currentWaypoint, 0.1f);
			for (int i = targetIndex + 1; i < path.Length; i++) {
				Gizmos.DrawLine (path[i - 1], path [i]);
				Gizmos.DrawSphere (path [i], 0.1f);
			}
		}
	}

	public void CheckPlayerCollision(){
		Character other = targetingBehaviour.GetPlayerCollision ();
		if (other != null) {
			other.KillMe ();
		}
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "ForgiveTrap" && myFaction == Faction.Evil && trap == null) {
			GoodTrap goodTrap = other.GetComponent<GoodTrap> ();
			if (goodTrap.CanTriggerMe) {
				trap = goodTrap;
				currentWaypoint = trap.transform.position;
				onReachedTargetNode += OnReachedTrap;
			}
		}
	}

	private void OnReachedTrap(){
		if (trap.CanTriggerMe) {
			fsm.SetState (StateName.trapped);
			trap.Triggered (this, trapTime);
		} 
		else {
			trap = null;
		}
		onReachedTargetNode -= OnReachedTrap;
	}

	public void ChangeStrategy(){
		patrolState.InitWaypoints (friendlyTargeting.patrolPoints);
		targetingBehaviour.UpdateTargetingBehaviour (friendlyTargeting);

		onTrapActivated ();

		GetComponent<SpriteRenderer> ().color = Color.yellow;
		//moveSpeed = 1.8f;

		trap = null;

		AIManager.Instance.ChangeAIState (this);
	}

	public override void KillMe (){
		AIManager.Instance.ChangeAIState (this);
		StartCoroutine (KillSelfOverTime());
		base.KillMe ();
	}

	private IEnumerator KillSelfOverTime(){
		SpriteRenderer r = GetComponent<SpriteRenderer> ();
		Color deadC = r.color;
		deadC.a = 0.7f;
		r.color = deadC;
		yield return new WaitForSeconds (1f);
		Destroy (gameObject);
	}
}

public enum PathState{
	following,
	requested,
	none
}