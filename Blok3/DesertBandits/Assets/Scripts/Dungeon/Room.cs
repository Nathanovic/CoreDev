using System;
using System.Collections.Generic;
using UnityEngine;

//holds all relevant information for a room
public class Room : IComparable<Room>{
	public List<Node> tiles;
	public List<Node> edgeTiles;
	public List<Room> connectedRooms;
	public int roomSize;
	public bool isMainRoom;
	public bool isAccessibleFromMainRoom;

	public List<List<Node>> passages;

	public Room(){}

	public Room(List<Node> roomTiles, int[,] map){
		tiles = roomTiles;
		roomSize = tiles.Count;

		connectedRooms = new List<Room>();
		edgeTiles = new List<Node>();
		passages = new List<List<Node>> ();

		//get all of the edgeTiles:
		foreach(Node tile in tiles){
			for(int x = tile.tileX - 1; x <= tile.tileX + 1; x ++){
				for(int y = tile.tileY - 1; y <= tile.tileY + 1; y ++){
					if(x == tile.tileX || y == tile.tileY){//exclude diagonal neighbours
						if(map[x,y] == 1){
							tile.accessibility++;
							edgeTiles.Add(tile);
						}
					}
				}
			}
		}
	}

	//make sure our passage tiles are set correctly:
	public void SetPassageTiles(Node passageCenter, int passageRadius){
		passageCenter.accessibility += 3;
		List<Node> passage = new List<Node> ();
		passage.Add (passageCenter);//make sure the center node is easy to find
		foreach (Node tile in edgeTiles) {
			if (passage.Contains (tile))
				continue;
			
			int xDiff = Mathf.Abs (tile.tileX - passageCenter.tileX);
			int yDiff = Mathf.Abs (tile.tileY - passageCenter.tileY);

			if (xDiff <= passageRadius && yDiff <= passageRadius) {
				tile.accessibility += 2;
				passage.Add (tile);
			}
		}

		passages.Add (passage);
	}

	//update the connectivity of all this room and all connected rooms:
	public void SetAccessibleFromMainRoom(){
		if (!isAccessibleFromMainRoom) {
			isAccessibleFromMainRoom = true;
			foreach (Room connectedRoom in connectedRooms) {
				connectedRoom.SetAccessibleFromMainRoom ();
			}
		}
	}

	public static void ConnectRooms(Room roomA, Room roomB){
		if (roomA.isAccessibleFromMainRoom) {
			roomB.SetAccessibleFromMainRoom ();
		}
		else if(roomB.isAccessibleFromMainRoom) {
			roomA.SetAccessibleFromMainRoom ();
		}

		roomA.connectedRooms.Add (roomB);
		roomB.connectedRooms.Add (roomA);
	}

	public bool IsConnected(Room otherRoom){
		return connectedRooms.Contains(otherRoom);
	}

	//we will sort the rooms based on their size:
	public int CompareTo (Room other) {
		return other.roomSize.CompareTo (roomSize);
	}
}