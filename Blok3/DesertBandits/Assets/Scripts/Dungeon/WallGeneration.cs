using System.Collections.Generic;
using UnityEngine;

//used to generate walls from a flat mesh
//this is done by detecting the inner outlines of the flat mesh and then 'extruding' this downwards
//also creates the floor
public class WallGeneration : MonoBehaviour {

	[SerializeField]private MeshFilter walls;

	private MeshData meshData;

	private List<Vector3> vertices{
		get{ 
			return meshData.vertices;
		}
		set{ 
			meshData.vertices = value;
		}
	}

	private Dictionary<int, List<Triangle>> triangleDictionary{
		get{ 
			return meshData.triangleDictionary;
		}
		set{ 
			meshData.triangleDictionary = value;
		}
	}
	private List<List<int>> outlines{
		get{ 
			return meshData.outlines;
		}
		set{ 
			meshData.outlines = value;
		}
	}
	private HashSet<int> checkedVertices{
		get{ 
			return meshData.checkedVertices;
		}
		set{ 
			meshData.checkedVertices = value;
		}
	}

	public void ClearMesh(){
		walls.mesh.Clear ();
	}

	public void GenerateWallMesh(MeshData _meshData, int dungeonHeight){
		meshData = _meshData;
		CalculateMeshOutlines ();

		List<Vector3> wallVertices = new List<Vector3> ();
		List<int> wallTriangles = new List<int> ();
		Mesh wallMesh = new Mesh ();
		Vector3 wallOffset = Vector3.up * -dungeonHeight;

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

		MeshCollider coll = walls.gameObject.GetComponent<MeshCollider> ();
		coll.sharedMesh = wallMesh;
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
}
