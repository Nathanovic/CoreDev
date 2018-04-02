using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// this script generates a random map using the cellular automata rules
public class MapGenerator : MonoBehaviour {

	private DungeonMaster dungeonMaster;
	private MeshData meshData;

	private List<Room> allRooms;

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;

	public int dungeonHeight = 5;
	public int[,] map;//0 === empty, 1 === wall
	public int borderSize = 5;
	private int[,] borderedMap;

	public int smoothValue = 5;
	public int wallThresholdSize = 5;
	public int roomThresholdSize = 100;
	public int passageRadius = 2;

	void Start(){
		dungeonMaster = GetComponent<DungeonMaster> ();
		meshData = new MeshData ();
		meshData.dungeonSize = new Vector2 (width, height);
		GenerateMap ();
	}

	void Update(){
		if (Input.GetKeyUp (KeyCode.Return)) {
			GenerateMap ();
		}
	}

	void GenerateMap(){
		map = new int[width, height];
		meshData.Clear (); 
		RandomFillMap ();

		WallGeneration wallGenn = GetComponent<WallGeneration> ();
		wallGenn.ClearMesh ();

		for (int i = 0; i < smoothValue; i++) {
			DoActionForAllTiles (SmoothTile);
		}

		ProcessMap ();

		//add a border to the map:
		borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength (0); x++) {
			for (int y = 0; y < borderedMap.GetLength (1); y++) {
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)//x inside of the map(it's not a border) 
					borderedMap [x, y] = map [x - borderSize, y - borderSize];
				else
					borderedMap [x, y] = 1;
			}
		}

		//generate the mesh:
		FlatMeshGenerator meshGen = GetComponent<FlatMeshGenerator> ();
		meshGen.GenerateFlatMesh (meshData, borderedMap, 1f, dungeonHeight);

		WallGeneration wallGen = GetComponent<WallGeneration> ();
		wallGenn.ClearMesh ();
		wallGen.GenerateWallMesh (meshData, dungeonHeight);

		dungeonMaster.FillRooms (map, allRooms, dungeonHeight, passageRadius);
	} 
		
	void ProcessMap(){
		List<List<Node>> wallRegions = GetRegions (1);

		foreach (List<Node> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				foreach (Node tile in wallRegion) {
					map [tile.tileX, tile.tileY] = 0;
				}
			}
		}

		List<List<Node>> roomRegions = GetRegions (0);
		List<Room> survivingRooms = new List<Room> ();

		foreach (List<Node> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) {
				foreach (Node tile in roomRegion) {
					map [tile.tileX, tile.tileY] = 1;
				}
			}
			else {
				survivingRooms.Add (new Room (roomRegion, map));
			}
		}

		survivingRooms.Sort ();
		survivingRooms [0].isMainRoom = true;
		survivingRooms [0].isAccessibleFromMainRoom = true;

		ConnectClosestRooms (survivingRooms);

		allRooms = survivingRooms;
	}

	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false){
		List<Room> roomListA = new List<Room> ();//not accessible roms
		List<Room> roomListB = new List<Room> ();//accessible rooms

		if (forceAccessibilityFromMainRoom) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add (room);
				}
				else {
					roomListA.Add (room);
				}
			}
		}
		else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Node bestTileA = new Node ();
		Node bestTileB = new Node ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;
				if (roomA.connectedRooms.Count > 0) {
					continue;
				}
			}

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected(roomB)) {
					continue;
				}

				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
						Node tileA = roomA.edgeTiles [tileIndexA];
						Node tileB = roomB.edgeTiles [tileIndexB];
						int xDist = tileA.tileX - tileB.tileX;
						int yDist = tileA.tileY - tileB.tileY;
						int distanceBetweenRooms = xDist * xDist + yDist * yDist;
						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							possibleConnectionFound = true;
							bestDistance = distanceBetweenRooms;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}

			if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
				CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms (allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms (allRooms, true);
		}
	}

	void CreatePassage(Room roomA, Room roomB, Node tileA, Node tileB){
		roomA.SetPassageTiles (tileA, passageRadius);
		roomB.SetPassageTiles (tileB, passageRadius);
		Room.ConnectRooms (roomA, roomB);

		List<Node> line = GetLine (tileA, tileB);
		foreach (Node tile in line) {
			ClearCircle (tile);
		}
	}

	void ClearCircle(Node tile){
		for(int x = -passageRadius; x < passageRadius; x ++){
			for(int y = -passageRadius; y < passageRadius; y ++){
				int drawX = tile.tileX + x;
				int drawY = tile.tileY + y;
				if (IsInMapRange (drawX, drawY)) {
					map [drawX, drawY] = 0;
				}
			}
		}
	}

	List<Node> GetLine(Node from, Node to){
		List<Node> line = new List<Node> ();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs (dy);
			shortest = Mathf.Abs (dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i = 0; i < longest; i++) {
			line.Add (new Node (x, y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				}
				else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	//get all regions of tiletype using the floodfill method:
	List<List<Node>> GetRegions(int tileType){
		List<List<Node>> regions = new List<List<Node>> ();
		int[,] checkedMap = new int[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (checkedMap [x, y] == 0 && map[x,y] == tileType) {
					List<Node> newRegion = GetRegionTiles (x, y);
					regions.Add(newRegion);

					foreach (Node tile in newRegion) {
						checkedMap [tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	//get region tiles using the floodfill method:
	List<Node> GetRegionTiles(int startX, int startY){
		List<Node> tiles = new List<Node> ();
		int[,] checkedMap = new int[width, height];//the tiles we've already looked at
		int typeTyle = map [startX, startY];

		Queue<Node> queue = new Queue<Node> ();
		queue.Enqueue (new Node (startX, startY));
		checkedMap [startX, startY] = 1;

		while (queue.Count > 0) {
			Node tile = queue.Dequeue ();
			tiles.Add (tile);

			for(int x = tile.tileX - 1; x <= tile.tileX + 1; x ++){
				for(int y = tile.tileY - 1; y <= tile.tileY + 1; y ++){
					if (IsInMapRange (x, y) && (x == tile.tileX || y == tile.tileY)) {//do action for neighbour(not diagonal)
						if(checkedMap[x,y] == 0 && map[x,y] == typeTyle){
							checkedMap [x, y] = 1;
							queue.Enqueue(new Node(x, y));
						}
					}
				}
			}
		}

		return tiles;
	}

	bool IsInMapRange(int x, int y){
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	void RandomFillMap(){
		if (useRandomSeed) {
			seed = Time.time.ToString ();
		}

		//use pseudorandom values to fill the map randomly:
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
					map [x, y] = 1;
				else
					map [x, y] = (pseudoRandom.Next (0, 100) < randomFillPercent) ? 1 : 0;
			}
		}
	}

	void SmoothTile(int x, int y){
		int neighbourWallTiles = GetSurroundingWallCount (x, y);
		if (neighbourWallTiles > 4) {
			map [x, y] = 1;
		}
		else if(neighbourWallTiles < 4) {
			map [x, y] = 0;
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY){
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
				if (IsInMapRange(neighbourX, neighbourY)) {
					if (neighbourX == gridX && neighbourY == gridY)
						continue;

					wallCount += map [neighbourX, neighbourY];
				}
				else {
					wallCount++;
				}
			}
		}

		return wallCount;
	}

	void DoActionForAllTiles(UnityAction<int, int> tileCallback){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tileCallback (x, y);
			}
		}
	}

	void OnDrawGizmos(){
		if (!Application.isPlaying) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube (Vector3.zero, new Vector3 (width, 1f, height));
		}
	}
}

public class MeshData{
	public Vector2 dungeonSize;
	public List<Vector3> vertices;
	public List<int> triangles;

	public Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();//key => vertex index, value => all triangles that belong to it
	public List<List<int>> outlines = new List<List<int>>();
	public HashSet<int> checkedVertices = new HashSet<int> ();//this is used to prevent double outline-checks for a vertex

	public void Clear(){
		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		triangleDictionary.Clear ();
		outlines.Clear ();
		checkedVertices.Clear ();		
	}
}

public struct Triangle {
	private int[] vertices;

	public Triangle(int a, int b, int c){
		vertices = new int[3];
		vertices[0] = a;
		vertices[1] = b;
		vertices[2] = c;
	}

	public bool Contains(int vertexIndex){
		return vertexIndex == vertices[0] || vertexIndex == vertices[1] || vertexIndex == vertices[2];
	}

	//make sure we can get our vertices like from an array:
	public int this[int i]{
		get{ 
			return vertices [i];
		}
	}
}

public class Node{
	public int tileX, tileY;
	public int accessibility;//used to measure how likely it is that something will be placed here

	public Node(){}

	public Node(int x, int y){
		tileX = x;
		tileY = y;
	}
}
