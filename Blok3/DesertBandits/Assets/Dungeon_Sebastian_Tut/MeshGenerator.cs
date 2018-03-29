﻿using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

	public float wallHeight = 5f;
	public MeshFilter walls;

	public SquareGrid squareGrid;
	private List<Vector3> vertices;
	private List<int> triangles;

	private Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();//key => vertex index, value => all triangles that belong to it
	private List<List<int>> outlines = new List<List<int>>();
	private HashSet<int> checkedVertices = new HashSet<int> ();//this is used to prevent double outline-checks for a vertex

	public void GenerateMesh(int[,] map, float squareSize){
		triangleDictionary.Clear ();
		outlines.Clear ();
		checkedVertices.Clear ();

		squareGrid = new SquareGrid (map, squareSize);

		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		for(int x = 0; x < squareGrid.squares.GetLength(0); x ++){
			for(int y = 0; y < squareGrid.squares.GetLength(1); y ++){
				TriangulateSquare (squareGrid.squares [x, y]);
			}
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals ();
		GetComponent<MeshFilter> ().mesh = mesh;

		CreateWallMesh ();
	}

	void CreateWallMesh(){
		CalculateMeshOutlines ();

		List<Vector3> wallVertices = new List<Vector3> ();
		List<int> wallTriangles = new List<int> ();
		Mesh wallMesh = new Mesh ();
		Vector3 wallOffset = Vector3.up * -wallHeight;

		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count - 1; i++) {
				int startIndex = wallVertices.Count;
				wallVertices.Add(vertices[outline[i]]);//left
				wallVertices.Add(vertices[outline[i + 1]]);//right
				wallVertices.Add(vertices[outline[i]] + wallOffset);//bottom left
				wallVertices.Add(vertices[outline[i + 1]] + wallOffset);//bottomm right

				wallTriangles.Add (startIndex + 0);//top left
				wallTriangles.Add (startIndex + 3);//bottom right
				wallTriangles.Add (startIndex + 2);//bottom left

				wallTriangles.Add (startIndex + 3);//bottom right
				wallTriangles.Add (startIndex + 0);//top right
				wallTriangles.Add (startIndex + 1);//top left

				//debug draw it:
				/*
				Debug.DrawLine (wallVertices [startIndex], wallVertices [startIndex + 2], Color.blue, 10f);
				Debug.DrawLine (wallVertices [startIndex + 3], wallVertices [startIndex], Color.blue, 10f);
				*/
			}
		}

		wallMesh.vertices = wallVertices.ToArray ();
		wallMesh.triangles = wallTriangles.ToArray ();
		walls.mesh = wallMesh;
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
			checkedVertices.Add (square.topLeft.vertexIndex);
			checkedVertices.Add (square.topRight.vertexIndex);
			checkedVertices.Add (square.bottomRight.vertexIndex);
			checkedVertices.Add (square.bottomLeft.vertexIndex);
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
		if (triangleDictionary.ContainsKey (vertexIndexKey)) {
			triangleDictionary [vertexIndexKey].Add (triangle);
		} 
		else {
			List<Triangle> triangleList = new List<Triangle> ();
			triangleList.Add (triangle);
			triangleDictionary.Add (vertexIndexKey, triangleList);
		}
	}

	void CalculateMeshOutlines(){
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains (vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex (vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add (newOutlineVertex);

					List<int> newOutline = new List<int> ();
					newOutline.Add (vertexIndex);
					outlines.Add (newOutline);
					FollowOutline (newOutlineVertex, outlines.Count - 1);
					outlines [outlines.Count - 1].Add (vertexIndex);
				}
			}
		}
	}
		
	void FollowOutline(int vertexIndex, int outlineIndex){
		outlines [outlineIndex].Add (vertexIndex);
		checkedVertices.Add (vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex (vertexIndex);
		if (nextVertexIndex != -1) {
			FollowOutline (nextVertexIndex, outlineIndex);
		}
	}

	//find the outline vertex connected to a vertex and return it (return -1 if there is none)
	int GetConnectedOutlineVertex(int vertexIndex){
		List<Triangle> trianglesContainingVertex = triangleDictionary [vertexIndex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex [i];

			for (int j = 0; j < 3; j++) {
				int vertexB = triangle [j];
				if (vertexB == vertexIndex || checkedVertices.Contains(vertexB))
					continue;
				
				if (IsOutlineEdge (vertexIndex, vertexB)) {
					return vertexB;
				}
			}
		}

		return -1;
	}

	//check if two vertices form an outline edge
	bool IsOutlineEdge(int vertexA, int vertexB){
		List<Triangle> trianglesContainingVertexA = triangleDictionary [vertexA];
		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
			if (trianglesContainingVertexA [i].Contains (vertexB)) {
				sharedTriangleCount++;
				if (sharedTriangleCount > 1) {
					break;
				}
			}
		}

		return sharedTriangleCount == 1;
	}

	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		private int[] vertices;

		public Triangle(int a, int b, int c){
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;

			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}

		//make sure we can get our vertices like from an array:
		public int this[int i]{
			get{ 
				return vertices [i];
			}
		}

		public bool Contains(int vertexIndex){
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}

	//this grid holds a 2 dimensional array of squares
	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize){
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

			for(int x = 0; x < nodeCountX; x ++){
				for(int y = 0; y < nodeCountY; y ++){
					Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);
					controlNodes[x,y] = new ControlNode(pos, map[x,y] == 1, squareSize);
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

		public ControlNode(Vector3 _pos, bool _active, float squareSize): base(_pos){
			active = _active;
			above = new Node(position + Vector3.forward * squareSize/2f);
			right = new Node(position + Vector3.right * squareSize/2f);
		}
	}

	public bool showGizmosGrid;
	void OnDrawGizmos(){
		if (squareGrid != null && showGizmosGrid) {
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