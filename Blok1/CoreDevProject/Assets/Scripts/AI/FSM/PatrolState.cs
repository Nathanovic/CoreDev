using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PatrolState : State {

	public Transform[] startingWaypoints;
	private Vector2[] waypoints;
	private List<int> patrollableWaypointIndexes = new List<int>();
	private int waypointIndex = -1;

	public override void Init(AI _target){
		base.Init(_target);
		InitWaypoints (startingWaypoints);
	}

	public void InitWaypoints(Transform[] _patrolWaypoints){
		int waypointCount = _patrolWaypoints.Length;
		waypoints = new Vector2[waypointCount];
		patrollableWaypointIndexes.Clear ();
		for(int i = 0; i < waypointCount; i ++){
			patrollableWaypointIndexes.Add (i);
			waypoints[i] = _patrolWaypoints [i].GetPosition ();
		}
		waypointIndex = -1;
	}

	public void PatrollingSetup(bool fromStartCall){
		//prevent patrolling to the waypoint the AI is already at:
		int rndmIndx = Random.Range (0, patrollableWaypointIndexes.Count);
		int rndmWaypointIndx = patrollableWaypointIndexes [rndmIndx];

		if (waypointIndex != -1) {
			patrollableWaypointIndexes.Add (waypointIndex);//deze kan de volgende keer weer gebruikt worden
		}
		waypointIndex = rndmWaypointIndx;
		patrollableWaypointIndexes.RemoveAt (rndmIndx);//deze kan de volgende keer niet gekozen worden

		//vanaf hier gaat de rest van het lopen in de AI class
		baseAI.RequestPath (waypoints [waypointIndex], fromStartCall);
	}

	//state implementation:
	public override void Start (){
		PatrollingSetup (true);
	}
	public override void Run(){
		baseAI.FollowPath ();

		if (baseAI.CurrentPathState == PathState.none) {
			PatrollingSetup (false);
		}
		if (baseAI.chaseState.CanChaseTarget ()) {
			onState (StateName.chasing);
		}
	}
}