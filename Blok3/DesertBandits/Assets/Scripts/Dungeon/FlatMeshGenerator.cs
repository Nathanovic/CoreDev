using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//this script ensures that we generate a flat mesh from the grid that we generated in the mapGenerator script
public class FlatMeshGenerator : MonoBehaviour {

	private MeshData meshData;
	[SerializeField]private MeshFilter roofMeshFilter;
	[SerializeField]private MeshFilter navMeshSurfacFilter;

	private List<Vector3> vertices{
		get{ 
			return meshData.vertices;
		}
		set{ 
			meshData.vertices = value;
		}
	}
	private List<int> triangles{
		get{ 
			return meshData.triangles;
		}
		set{ 
			meshData.triangles = value;
		}
	}

	private SquareGrid squareGrid;

	public void GenerateFlatMesh(MeshData _meshData, int[,] map, float squareSize, float dungeonHeight){
		//create the roof mesh:
		meshData = _meshData;
		squareGrid = new SquareGrid (map, squareSize, 0f, 1);
		Mesh flatMesh = CreateMeshFromData ();
		roofMeshFilter.mesh = flatMesh;

		//create the navmesh surface:
		meshData = new MeshData ();
		meshData.Clear ();
		squareGrid = new SquareGrid (map, squareSize, dungeonHeight, 0);
		Mesh nmMesh = CreateMeshFromData ();
		navMeshSurfacFilter.mesh = nmMesh;

		NavMeshSurface nmSurface = navMeshSurfacFilter.gameObject.GetComponent<NavMeshSurface> ();
		roofMeshFilter.gameObject.SetActive (false);
		nmSurface.BuildNavMesh ();
		roofMeshFilter.gameObject.SetActive (true);
		MeshCollider floorColl = nmSurface.gameObject.AddComponent<MeshCollider> ();
		floorColl.sharedMesh = nmMesh;
	}

	private Mesh CreateMeshFromData(){
		for(int x = 0; x < squareGrid.squares.GetLength(0); x ++){
			for(int y = 0; y < squareGrid.squares.GetLength(1); y ++){
				TriangulateSquare (squareGrid.squares [x, y]);
			}
		}

		Mesh flatMesh = new Mesh ();
		flatMesh.vertices = vertices.ToArray();
		flatMesh.triangles = triangles.ToArray();
		flatMesh.RecalculateNormals ();

		return flatMesh;
	}

	//convert the square data into tri's using the square configuration number
	void TriangulateSquare(Square square){
		switch (square.configuration) {
		case 0:
			break;
		
			//1 point:
		case 1:
			MeshFromPoints (square.centreLeft, square.centreBottom, square.bottomLeft);
			break;
		case 2:
			MeshFromPoints (square.bottomRight, square.centreBottom, square.centreRight);
			break;
		case 4:
			MeshFromPoints (square.topRight, square.centreRight, square.centreTop);
			break;
		case 8:
			MeshFromPoints (square.topLeft, square.centreTop, square.centreLeft);
			break;

			//2 points:
		case 3:
			MeshFromPoints (square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 6:
			MeshFromPoints (square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
			break;
		case 9:
			MeshFromPoints (square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints (square.topLeft, square.topRight, square.centreRight, square.centreLeft);
			break;
		case 5:
			MeshFromPoints (square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
			break;
		case 10:
			MeshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomLeft, square.centreBottom, square.centreLeft);
			break;

			//3 points:
		case 7:
			MeshFromPoints (square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 11:
			MeshFromPoints (square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			MeshFromPoints (square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;

			//4 points:
		case 15:
			MeshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			//prevent checking the outline for these vertices:
			meshData.checkedVertices.Add (square.topLeft.vertexIndex);
			meshData.checkedVertices.Add (square.topRight.vertexIndex);
			meshData.checkedVertices.Add (square.bottomRight.vertexIndex);
			meshData.checkedVertices.Add (square.bottomLeft.vertexIndex);
			break;
		}
	}

	void MeshFromPoints(params Node[] points){
		AssignVertices (points);

		if (points.Length >= 3)
			CreateTriangle (points [0], points [1], points [2]);
		if (points.Length >= 4)
			CreateTriangle (points [0], points [2], points [3]);
		if (points.Length >= 5)
			CreateTriangle (points [0], points [3], points [4]);
		if (points.Length >= 6)
			CreateTriangle (points [0], points [4], points [5]);
	}

	void AssignVertices(params Node[] points){
		for (int i = 0; i < points.Length; i++) {
			if (points [i].vertexIndex == -1) {
				points [i].vertexIndex = vertices.Count;
				vertices.Add (points [i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c){
		triangles.Add (a.vertexIndex);
		triangles.Add (b.vertexIndex);
		triangles.Add (c.vertexIndex);

		Triangle triangle = new Triangle (a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary (a.vertexIndex, triangle);
		AddTriangleToDictionary (b.vertexIndex, triangle);
		AddTriangleToDictionary (c.vertexIndex, triangle);
	}

	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle){
		//prevent double keys in the dictionary:
		if (meshData.triangleDictionary.ContainsKey (vertexIndexKey)) {
			meshData.triangleDictionary [vertexIndexKey].Add (triangle);
		} 
		else {
			List<Triangle> triangleList = new List<Triangle> ();
			triangleList.Add (triangle);
			meshData.triangleDictionary.Add (vertexIndexKey, triangleList);
		}
	}

	//this grid holds a 2 dimensional array of squares
	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize, float dungeonHeight, int mapType = 1){
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

			for(int x = 0; x < nodeCountX; x ++){
				for(int y = 0; y < nodeCountY; y ++){
					Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, -dungeonHeight, -mapHeight/2 + y * squareSize + squareSize/2);
					controlNodes[x,y] = new ControlNode(pos, map[x,y] == mapType, squareSize);
				}	
			}

			//create a grid of squares out of the control nodes:
			squares = new Square[nodeCountX - 1, nodeCountY - 1];

			for(int x = 0; x < nodeCountX - 1; x ++){
				for(int y = 0; y < nodeCountY - 1; y ++){
					squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x + 1, y+1], controlNodes[x+1,y], controlNodes[x,y]);
				}	
			}
		}
	}

	//a square contains all mesh data for a tile in the dungeon:
	public class Square{
		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centreTop, centreRight, centreBottom, centreLeft;
		public int configuration;

		public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft){
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centreTop = topLeft.right;
			centreRight = bottomRight.above;
			centreBottom = bottomLeft.right;
			centreLeft = bottomLeft.above;

			if(topLeft.active)
				configuration += 8;
			if(topRight.active)
				configuration += 4;
			if(bottomRight.active)
				configuration += 2;
			if(bottomLeft.active)
				configuration += 1;
		}
	}

	//one of the points of the marching square
	public class Node{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos){
			position = _pos;
		}
	}

	//a cornder-node (contains the node up and right of itself as nodes
	public class ControlNode : Node{

		public bool active;//true == wall; false == open
		public Node above, right;

		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos){
			active = _active;
			above = new Node(position + Vector3.forward * squareSize/2f);
			right = new Node(position + Vector3.right * squareSize/2f);
		}
	}

	public bool showGizmos;
	void OnDrawGizmos(){
		if (squareGrid != null && showGizmos) {
			for(int x = 0; x < squareGrid.squares.GetLength(0); x ++){
				for(int y = 0; y < squareGrid.squares.GetLength(1); y ++){
					Gizmos.color = squareGrid.squares [x, y].topLeft.active ? Color.black : Color.white;
					Gizmos.DrawCube (squareGrid.squares [x, y].topLeft.position, Vector3.one * .4f);

					Gizmos.color = squareGrid.squares [x, y].topRight.active ? Color.black : Color.white;
					Gizmos.DrawCube (squareGrid.squares [x, y].topRight.position, Vector3.one * .4f);

					Gizmos.color = squareGrid.squares [x, y].bottomRight.active ? Color.black : Color.white;
					Gizmos.DrawCube (squareGrid.squares [x, y].bottomRight.position, Vector3.one * .4f);

					Gizmos.color = squareGrid.squares [x, y].bottomLeft.active ? Color.black : Color.white;
					Gizmos.DrawCube (squareGrid.squares [x, y].bottomLeft.position, Vector3.one * .4f);

					Gizmos.color = Color.grey;
					Gizmos.DrawCube (squareGrid.squares [x, y].centreTop.position, Vector3.one * .2f);
					Gizmos.DrawCube (squareGrid.squares [x, y].centreRight.position, Vector3.one * .2f);
					Gizmos.DrawCube (squareGrid.squares [x, y].centreBottom.position, Vector3.one * .2f);
					Gizmos.DrawCube (squareGrid.squares [x, y].centreLeft.position, Vector3.one * .2f);
				}	
			}
		}
	}
}
