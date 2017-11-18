using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

	private PathRequestManager requestManager;
	private Grid grid;

	void Awake(){
		grid = GetComponent<Grid> ();
		requestManager = GetComponent<PathRequestManager> ();
	}

	public void StartFindPath(Vector2 seekerPos, Vector2 targetPos){
		StartCoroutine(FindPath (seekerPos, targetPos));
	}

	IEnumerator FindPath(Vector2 startPos, Vector2 endPos){
		Vector2[] waypoints = new Vector2[0];
		bool pathSucces = false;

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (endPos);

		if(startNode.walkable && targetNode.walkable && startNode != targetNode){
			Heap<Node> openSet = new Heap<Node> (grid.GridSize);
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode) {
					pathSucces = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
						}
						else {
							openSet.UpdateItem (neighbour);
						}
					}
				}
			}
		}

		yield return null;

		if(pathSucces){
			waypoints = RetracedPath (startNode, targetNode);
			//pathLength = Vector2.Distance (startPos, waypoints[0]);
			//pathLength += RetracedPathLength (waypoints);
		}
		requestManager.FinishedProcessingPath (waypoints, pathSucces);
	}

	Vector2[] RetracedPath(Node startNode, Node endNode){
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		while(currentNode != startNode){
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();
		Vector2[] waypoints = SimplifyPath (path);

		return waypoints;
	}

	Vector2[] SimplifyPath(List<Node> path){
		List<Vector2> waypoints = new List<Vector2> ();
		Vector2 directionOld = Vector2.zero;

		int lastIndx = path.Count - 1;

		for(int i = 0; i < lastIndx; i ++){
			Vector2 directionNext = new Vector2 (path [i + 1].gridX - path [i].gridX, path [i + 1].gridY - path [i].gridY);
			if(directionNext != directionOld){
				directionOld = directionNext;
				waypoints.Add (path[i].worldPosition);
			}
		}
		waypoints.Add(path[lastIndx].worldPosition);//laatste punt van pad moer er sws in komen

		return waypoints.ToArray();
	}

	float RetracedPathLength(Vector2[] waypoints){
		float pathLength = 0;
		for(int i = 1; i < waypoints.Length; i ++){
			float dist = Vector2.Distance (waypoints [i], waypoints [i - 1]);
			pathLength += dist;
		}
		return pathLength;
	}

	int GetDistance(Node nodeA, Node nodeB){
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		}
		return 14 * dstX + 10 * (dstY - dstX);
	}
}