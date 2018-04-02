using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

/*
// this script generates a random map using the cellular automata rules
public class MapGeneratorDebugger : MonoBehaviour {

	[SerializeField]private DungeonMaster dungeonMaster;
	private MeshData meshData;

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;

	public int[,] map;//0 === empty, 1 === wall
	public int borderSize = 5;
	private int[,] borderedMap;

	public int smoothValue = 5;
	public int wallThresholdSize = 5;
	public int roomThresholdSize = 100;
	public int passageRadius = 2;


	[Header("Debug values: ")]
	public int generationProgress = 1;
	public float smoothMapTime = 0.2f;
	public float generationTime = 2f;
	public List<Node> invalidTiles = new List<Node>();
	public bool drawGizmos;
	private bool isRunning;
	private int generationIndex;

	void Start(){
		meshData = new MeshData ();
		meshData.dungeonSize = new Vector2 (width, height);
		StartCoroutine(GenerateMap ());
	}

	void Update(){
		if (Input.GetKeyUp (KeyCode.Space)) {
			StartCoroutine(GenerateMap ());
		}
	}

	IEnumerator GenerateMap(){
		if (isRunning)
			yield break;

		isRunning = true;
		generationIndex = 1;

		map = new int[width, height];
		meshData.Clear (); 
		RandomFillMap ();

		WallGeneration wallGenn = GetComponent<WallGeneration> ();
		wallGenn.ClearMesh ();
		invalidTiles.Clear ();

		for (int i = 0; i < smoothValue; i++) {
			yield return new WaitForSeconds (smoothMapTime);
			DoActionForAllTiles (SmoothTile);
		}

		yield return new WaitForSeconds (smoothMapTime);
		generationIndex++;
		yield return ProcessMap ();
		generationIndex++;

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
		if (generationProgress > 1) {
			yield return new WaitForSeconds (generationTime);
			FlatMeshGenerator meshGen = GetComponent<FlatMeshGenerator> ();
			meshGen.GenerateFlatMesh (meshData, borderedMap, 1f);

			generationIndex++;
		}
		if (generationProgress > 2) {
			yield return new WaitForSeconds (generationTime);
			WallGeneration wallGen = GetComponent<WallGeneration> ();
			wallGen.GenerateWallMesh (meshData, 5);
		}

		generationIndex++;
		isRunning = false;
	} 
		
	IEnumerator ProcessMap(){
		List<List<Node>> wallRegions = GetRegions (1);

		foreach (List<Node> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				invalidTiles.AddRange(wallRegion);
				foreach (Node tile in wallRegion) {
					map [tile.tileX, tile.tileY] = 0;
				}
			}
		}

		List<List<Node>> roomRegions = GetRegions (0);
		List<Room> survivingRooms = new List<Room> ();

		foreach (List<Node> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) {
				invalidTiles.AddRange (roomRegion);
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

		yield return new WaitForSeconds (generationTime);
		invalidTiles.Clear ();
		yield return new WaitForSeconds (generationTime);

		ConnectClosestRooms (survivingRooms);
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

	public UnityEngine.UI.Text progressText;
	void LateUpdate(){
		progressText.text = "Step " + generationIndex.ToString ();		
	}

	void OnDrawGizmos(){
		if (!Application.isPlaying) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube (Vector3.zero, new Vector3 (width, 1f, height));
		}

		if (map == null || generationIndex == 0 || generationIndex > 3 || !drawGizmos)
			return;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
				if (generationIndex == 2) {	
					foreach (Node coord in invalidTiles) {
						if (coord.tileX == x && coord.tileY == y) {
							Gizmos.color = new Color (0.8f, 0.2f, 0.2f);
						}
					}
				}
				Vector3 pos = new Vector3 (-(float)width / 2f + x + .5f, 0f, -(float)height / 2f + y + .5f);
				Gizmos.DrawCube (pos, Vector3.one * 0.95f);
			}
		}
	}
}
*/