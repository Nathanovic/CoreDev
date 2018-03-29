using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TileType{
	Floor, Wall, Obstacle, Character
}

//this class creates the dungeon
public class DungeonCreator : MonoBehaviour {

	public GameObject floorPrefab, wallPrefab, filledWallPrefab;

	public float tileSize = 1f;
	public Coordinate dungeonStart;
	public Coordinate dungeonSize;

	public Coordinate dungeonBuildStart;

	public Tile[,] tiles;

	[Header("Rooms:")]
	public int roomCount;
	public List<CompositeRoom> compRooms = new List<CompositeRoom> ();
	public List<Room> rooms = new List<Room> ();

	public int roomWidthMax;
	public int roomWidthMin;

	private int roomID;

	void Start () {
		InitializeGrid ();
	}

	void InitializeGrid(){
		tiles = new Tile[dungeonSize.x, dungeonSize.y];

		for (int x = 0; x < dungeonSize.x; x++) {
			for (int y = 0; y < dungeonSize.y; y++) {
				Coordinate tilePosition = new Coordinate (x, y);
				Tile newTile = new Tile (tilePosition, tileSize);
				tiles [x, y] = newTile;
			}
		}

		dungeonBuildStart = new Coordinate(1,1);
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Space)) {
			foreach (Tile t in tiles) {
				t.DestroyObject ();
			}
			compRooms.Clear ();
			rooms.Clear ();

			StartCoroutine(CreateRooms ());
			//SpawnTiles ();
		}else if(Input.GetKeyUp(KeyCode.D)){
			AddRoomUnique (customRoom);
		}
	}

	public Room customRoom;//QQQ

	//create random rooms and make sure to make them composite if they overlap
	IEnumerator CreateRooms(){
		rooms = new List<Room> (roomCount);
		for (int i = 0; i < roomCount; i++) {
			//calculate roomvalues:
			Coordinate roomSize = new Coordinate (Random.Range (roomWidthMin, roomWidthMax), Random.Range (roomWidthMin, roomWidthMax));
			Coordinate roomMax = new Coordinate (dungeonSize.x - roomSize.x, dungeonSize.y - roomSize.y);
			Coordinate roomStart = Coordinate.Random (dungeonBuildStart, roomMax);

			//create the room:
			PerformActionForArea (roomStart, roomSize, MakeFloor);
			Room newRoom = new Room(this, roomStart, roomSize);
			newRoom.roomID = ++roomID;

			AddRoomUnique (newRoom);
			yield return new WaitForSeconds (0.5f);
		}
	}

	//make sure that we only have unique rooms in our room-lists:
	void AddRoomUnique(Room newRoom){
		List<Room> overlappedRooms = new List<Room> ();
		List<CompositeRoom> overlappedCompRooms = new List<CompositeRoom> ();
		for(int c = 0; c < compRooms.Count; c ++){
			Debug.Log ("comproom overlaps: " + compRooms [c].Overlap (newRoom).ToString());
			if (compRooms [c].Overlap (newRoom)) {
				overlappedCompRooms.Add (compRooms [c]);
			}
		}
		for (int r = 0; r < rooms.Count; r++) {
			rooms [r].TryMergeWithRoom (newRoom);
			if (rooms [r].isComposite) {
				overlappedRooms.Add (rooms [r]);
				rooms.Remove (rooms [r]);
			}
		}

		if (overlappedCompRooms.Count > 0) {
			CompositeRoom firstCompRoom = overlappedCompRooms [0];
			for (int c = 1; c < overlappedCompRooms.Count; c++) {
				CompositeRoom compRoom = overlappedCompRooms [c];
				firstCompRoom.MergeComposite (compRoom);
				compRooms.Remove (compRoom);
			}

			overlappedRooms.Add (newRoom);

			firstCompRoom.AddRooms (overlappedRooms);
		} 
		else if (overlappedRooms.Count > 0) {
			overlappedRooms.Add (newRoom);
			CompositeRoom newCompRoom = new CompositeRoom (overlappedRooms);
			compRooms.Add (newCompRoom);
		} 
		else {
			rooms.Add (newRoom);
		}

		PerformActionForArea (newRoom.start, newRoom.size, SpawnTile);//QQQ:
	}

	void MakeFloor(Tile tile){
		tile.type = TileType.Floor;
	}

	void SpawnTiles(){
		PerformActionForArea (Coordinate.ZeroCoord(), dungeonSize, SpawnTile);
	}

	void SpawnTile(Tile tile){
		GameObject prefab = null;
		switch (tile.type) {
		case TileType.Floor:
			prefab = floorPrefab;
			break;
		case TileType.Wall:
			prefab = wallPrefab;
			break;
		default:
			Debug.LogWarning ("unknown tiletype: " + tile.type.ToString ());
			break;
		}

		Vector3 position = TilePositionFromCoordinate (tile.coordinate);
		tile.tileObject = GameObject.Instantiate (prefab, position, Quaternion.identity, transform) as GameObject;
		tile.tileObject.transform.localScale = Vector3.one * tileSize;
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		Vector3 size = new Vector3 (dungeonSize.x * tileSize, dungeonSize.y * tileSize);
		Vector3 start = new Vector3 (dungeonStart.x * tileSize, dungeonStart.y * tileSize) + size / 2f;
		Gizmos.DrawWireCube (start, size);
		size = new Vector3 ((dungeonSize.x - 2) * tileSize, (dungeonSize.y - 2) * tileSize);
		Coordinate startCoo = Coordinate.Add(dungeonBuildStart, dungeonStart);
		start = new Vector3 (startCoo.x * tileSize, startCoo.y * tileSize) + size / 2f;
		Gizmos.DrawWireCube (start, size);

		Gizmos.color = Color.yellow;
		for(int i = 0; i < rooms.Count; i ++) {
			Room r = rooms [i];
			if (r.adjacentTiles.Count == 0)
				continue;

			foreach (Tile t in r.adjacentTiles) {
				Gizmos.DrawWireCube (TilePositionFromCoordinate (t.coordinate), Vector3.one * tileSize);
			}
		}

		Gizmos.color = Color.cyan;
		for (int c = 0; c < compRooms.Count; c++) {
			for (int i = 0; i < compRooms[c].connectedRooms.Count; i++) {
				Room r = compRooms[c].connectedRooms [i];
				if (r.adjacentTiles.Count == 0)
					continue;

				foreach (Tile t in r.adjacentTiles) {
					Gizmos.DrawWireCube (TilePositionFromCoordinate (t.coordinate), Vector3.one * tileSize);
				}
			}
		}
	}

	public void PerformActionForArea(Coordinate start, Coordinate size, UnityAction<Tile> tileAction){
		for (int x = start.x; x < start.x + size.x; x++) {
			for (int y = start.y; y < start.y + size.y; y++) {
				Tile tile = tiles [x, y];
				tileAction (tile);
			}
		}
	}

	public Vector3 TilePositionFromCoordinate(Coordinate coord){
		Vector3 worldPos = dungeonStart.Position () + coord.Position();
		worldPos *= tileSize;
		worldPos += new Vector3 (tileSize / 2f, tileSize / 2f);
		return worldPos;
	}
}

public class Tile{
	public TileType type;
	public float size;
	public Coordinate coordinate{ get; private set;}
	public GameObject tileObject;

	public Tile(Coordinate coord, float width){
		type = TileType.Wall;
		size = width;
		coordinate = coord;
	}

	public void DestroyObject(){
		if (tileObject != null) {
			GameObject.Destroy (tileObject);
		}
		type = TileType.Wall;
	}
}
