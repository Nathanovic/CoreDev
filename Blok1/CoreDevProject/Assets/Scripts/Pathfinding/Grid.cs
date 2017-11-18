using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	private Node[,] grid;

	private float nodeDiameter;
	private int gridSizeX;
	private int gridSizeY;

	public int GridSize{
		get{ 
			return gridSizeX * gridSizeY;
		}
	}

	[SerializeField]private bool displayGridGizmos;

	void Awake(){
		//how many nodes can we fit into our grid:
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);

		CreateGrid ();
	}

	void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		//positie voor bottom left gridNode:
		Vector2 worldBottomLeft = new Vector2(-gridSizeX * 0.5f, -gridSizeY * 0.5f);

		for(int x = 0; x < gridSizeX; x ++){
			for(int y = 0; y < gridSizeY; y ++){
				Vector2 worldPoint = worldBottomLeft 
					+ Vector2.right * (x * nodeDiameter + nodeRadius)
					+ Vector2.up * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius - 0.05f, unwalkableMask));
				grid [x, y] = new Node (walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node){
		List<Node> neighbours = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++){
				//don't add the node itself
				if (x == 0 && y == 0) {
					continue;
				}//dont add diagonal neighbours
				else if(x != 0 && y != 0){
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				//check if the node with these coords is within the grid
				if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node NodeFromWorldPoint(Vector2 worldPosition){
		float percentX = (worldPosition.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
		float percentY = (worldPosition.y + gridWorldSize.y * 0.5f) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return(grid[x,y]);
	}

	#region Accessed by characters:
	public Vector2 NodePosFromWorldPoint(Vector2 worldPosition){
		Node n = NodeFromWorldPoint (worldPosition);
		return n.worldPosition;
	}
	public Vector2 PositionFromInstruction(Vector2 playerPos, float x, float y){
		Node playerNode = NodeFromWorldPoint (playerPos);
		int nodeX = playerNode.gridX;
		int nodeY = playerNode.gridY;

		if (x != 0) {
			if (x > 0)
				nodeX ++;
			else
				nodeX --;

			if (nodeX >= 0 && nodeX < gridSizeX) {
				Node n = grid [nodeX, nodeY];
				if (n.walkable) {
					return n.worldPosition;
				}
			}
			nodeX = playerNode.gridX;
		}
		if (y != 0) {
			if (y > 0)
				nodeY ++;
			else
				nodeY --;

			if (nodeY >= 0 && nodeY < gridSizeY) {
				Node n = grid [nodeX, nodeY];
				if (n.walkable) {
					return n.worldPosition;
				}
			}
		}

		return playerPos;
	}
	#endregion

	void OnDrawGizmos(){
		Gizmos.DrawWireCube (transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));

		if (grid != null && displayGridGizmos) {
			foreach(Node n in grid){
				Color gizmC = (n.walkable) ? Color.green : Color.red;
				gizmC.a = 0.5f;

				Gizmos.color = gizmC;
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - 0.05f));
			}
		}
	}
}
