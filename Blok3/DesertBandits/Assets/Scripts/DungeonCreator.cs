using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//this class creates the dungeon
public class DungeonCreator : MonoBehaviour {

	public enum TileType{
		Floor, Wall, Obstacle, Character
	}

	public GameObject floorPrefab, wallPrefab, filledWallPrefab;

	public float tileSize = 1f;
	public Coordinate dungeonStart;
	public Coordinate dungeonSize;

	public Tile[,] tiles;

	[Header("Rooms:")]
	public int roomCount;
	public List<CompositeRoom> compRooms = new List<CompositeRoom> ();
	public List<Room> rooms = new List<Room> ();

	public int roomWidthMax;
	public int roomWidthMin;

	void Start () {
		InitializeGrid ();
	}

	void InitializeGrid(){
		tiles = new Tile[dungeonSize.x, dungeonSize.y];

		for (int x = 0; x < dungeonSize.x; x++) {
			for (int y = 0; y < dungeonSize.y; y++) {
				Coordinate tilePosition = new Coordinate (x, y);
				Tile newTile = new Tile (tilePosition);
				tiles [x, y] = newTile;
			}
		}
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Space)) {
			foreach (Tile t in tiles) {
				t.DestroyObject ();
			}
			compRooms.Clear ();
			rooms.Clear ();

			CreateRooms ();
			SpawnTiles ();
		}
	}

	//create random rooms and make sure to make them composite if they overlap
	void CreateRooms(){
		rooms = new List<Room> (roomCount);
		for (int i = 0; i < roomCount; i++) {
			//calculate roomvalues:
			Coordinate roomSize = new Coordinate (Random.Range (roomWidthMin, roomWidthMax), Random.Range (roomWidthMin, roomWidthMax));
			Coordinate roomMax = new Coordinate (dungeonSize.x - roomSize.x + 1, dungeonSize.y - roomSize.y + 1);
			Coordinate coordZero = Coordinate.ZeroCoord ();
			Coordinate roomStart = Coordinate.Random (Coordinate.ZeroCoord (), roomMax);

			//create the room:
			Room newRoom = new Room(roomStart, roomSize);
			//make sure that we only have unique rooms in our room-lists:
			List<Room> overlappedRooms = new List<Room> ();
			List<CompositeRoom> overlappedCompRooms = new List<CompositeRoom> ();
			for(int c = 0; c < compRooms.Count; c ++){
				if (compRooms [c].Overlap (newRoom)) {
					overlappedCompRooms.Add (compRooms [c]);
				}
			}
			for (int r = 0; r < rooms.Count; r++) {
				if (rooms [r].Overlap (newRoom)) {
					overlappedRooms.Add (rooms [r]);
				}
			}

			if (overlappedCompRooms.Count > 1) {
				CompositeRoom firstCompRoom = overlappedCompRooms [0];
				for (int c = 1; c < overlappedCompRooms.Count; c++) {
					CompositeRoom compRoom = overlappedCompRooms [c];
					firstCompRoom.MergeComposite (compRoom);
					compRooms.Remove (compRoom);
				}
					
				for (int r = 0; r < overlappedRooms.Count; r++) {
					rooms.Remove (overlappedRooms [r]);
				}
				overlappedRooms.Add (newRoom);

				firstCompRoom.AddRooms (overlappedRooms);
			} else if (overlappedRooms.Count > 0) {
				for (int r = 0; r < overlappedRooms.Count; r++) {
					rooms.Remove (overlappedRooms [r]);
				}
				overlappedRooms.Add (newRoom);
				CompositeRoom newCompRoom = new CompositeRoom (overlappedRooms);
				compRooms.Add (newCompRoom);
			} else {
				rooms.Add (newRoom);
			}

			PerformActionForArea (roomStart, roomSize, MakeFloor);
		}
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
	}

	void PerformActionForArea(Coordinate start, Coordinate size, UnityAction<Tile> tileAction){
		for (int x = start.x; x < start.x + size.x; x++) {
			for (int y = start.y; y < start.y + size.y; y++) {
				Tile tile = tiles [x, y];
				tileAction (tile);
			}
		}
	}

	public class Tile{
		public TileType type;
		public Coordinate coordinate{ get; private set;}
		public GameObject tileObject;

		public Tile(Coordinate coord){
			type = TileType.Wall;
			coordinate = coord;
		}

		public void DestroyObject(){
			if (tileObject != null) {
				Destroy (tileObject);
			}
			type = TileType.Wall;
		}
	}

	public Vector3 TilePositionFromCoordinate(Coordinate coord){
		Vector3 worldPos = dungeonStart.Position () + coord.Position();
		worldPos *= tileSize;
		worldPos += new Vector3 (tileSize / 2f, tileSize / 2f);
		return worldPos;
	}
}
