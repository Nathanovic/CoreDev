using System.Collections.Generic;
using UnityEngine;

//this script is used to place objects in the dungeon 
public class DungeonMaster : MonoBehaviour {

	private int[,] map;
	private Vector2 mapSize;
	private int dungeonHeight;
	private List<Room> rooms;

	public bool spawnPlayer;

	[SerializeField]private float deadEndRoomBlockedChance = 0.5f;
	[SerializeField]private float defaultRoomBlockedChance = 0.2f;

	[SerializeField]private GameObject playerPrefab;
	[SerializeField]private GameObject obstaclePrefab;

	[SerializeField]private Spawnable[] spawnables;

	public void FillRooms(int[,] _map, List<Room> allRooms, int _dungeonHeight, int passageRadius){
		map = _map;//i dont use this
		dungeonHeight = _dungeonHeight;
		mapSize = new Vector2(map.GetLength(0), map.GetLength(1));
		rooms = allRooms;

		if (spawnPlayer) {
			Node playerTile = rooms [0].tiles [0];
			Vector3 playerPos = CoordToWorldPos (playerTile);
			GameObject.Instantiate (playerPrefab, playerPos, Quaternion.identity);
		}

		Vector3 obstacleScale = new Vector3 (passageRadius * 2, dungeonHeight, passageRadius * 2);
		foreach (Room r in rooms) {
			float blockedChance = (r.connectedRooms.Count == 1 && !r.isMainRoom) ? deadEndRoomBlockedChance : defaultRoomBlockedChance;
			foreach (List<Node> passage in r.passages) {
				if (Random.value < blockedChance) {
					GameObject obstacle = GameObject.Instantiate (obstaclePrefab, CoordToWorldPos (passage [0]), Quaternion.identity, transform) as GameObject;
					obstacle.transform.localScale = obstacleScale;
				}
			}
		}

		List<Room> spawnRooms = allRooms;
		spawnRooms.Remove (allRooms [0]);

		foreach (Spawnable spawnable in spawnables) {
			SpawnRandom (spawnable, spawnRooms);
		}
	}

	//spawn random in rooms using recursion:
	void SpawnRandom(Spawnable spawnable, List<Room> availableRooms){
		if (availableRooms.Count == 0)
			return;
		
		int rndmRoomIndex = Random.Range (0, availableRooms.Count);
		int rndmTileIndex = Random.Range (0,availableRooms[rndmRoomIndex].tiles.Count);
		Node spawnTile = availableRooms [rndmRoomIndex].tiles [rndmTileIndex];

		Vector3 spawnPos = CoordToWorldPos (spawnTile);
		GameObject.Instantiate (spawnable.prefab, spawnPos, Quaternion.identity);

		spawnable.count--;

		if (spawnable.count > 0) {
			if (spawnable.maxOnePerRoom) {
				availableRooms.RemoveAt (rndmRoomIndex);
			}

			SpawnRandom (spawnable, availableRooms);
		}
	}

	void OnDrawGizmos(){
		if(rooms != null){
			foreach (Room r in rooms) {
				foreach (Node tile in r.edgeTiles) {
					Gizmos.color = new Color (tile.accessibility * 0.2f, 0, 0, 1f);
					Gizmos.DrawCube (CoordToWorldPos (tile), Vector3.one * 0.8f);
				}

				/*
				foreach (List<Node> passage in r.passages) {
					foreach (Node tile in passage) {
						Gizmos.color = Color.blue;
						Gizmos.DrawCube (CoordToWorldPos (tile), Vector3.one * 0.8f);
					}

					Gizmos.color = Color.white;
					Gizmos.DrawCube (CoordToWorldPos (passage [0]), Vector3.one);
				}
				*/
			}
		}
	}

	Vector3 CoordToWorldPos(Node tile){
		return new Vector3 (-(float)mapSize.x / 2f + 0.5f + tile.tileX, -(float)dungeonHeight / 2f, -(float)mapSize.y / 2f + 0.5f + tile.tileY);
	}

	[System.Serializable]
	public class Spawnable {
		public GameObject prefab;
		public int count = 3;
		public bool maxOnePerRoom = true;
	}
}
