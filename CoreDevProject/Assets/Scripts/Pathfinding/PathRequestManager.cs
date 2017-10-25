using UnityEngine;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour {

	private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
	private PathRequest currentPathRequest;

	private static PathRequestManager instance;
	private Pathfinding pathfinding;

	private bool isProcessingPath;

	void Awake(){
		instance = this;
		pathfinding = GetComponent<Pathfinding> ();
	}

	public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback){
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}

	void TryProcessNext(){
		if(!isProcessingPath && pathRequestQueue.Count > 0){
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessingPath = true;
			pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector2[] path, bool succes){
		currentPathRequest.callback (path, succes);
		isProcessingPath = false;
		TryProcessNext ();
	}

	struct PathRequest{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector2[], bool> callback;

		public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback){
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}
