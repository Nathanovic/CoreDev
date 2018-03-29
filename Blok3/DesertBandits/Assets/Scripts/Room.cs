using UnityEngine;
using System.Collections.Generic;

//contains all of the tiles that belong to this specific room
//these tiles are used to check for overlap or adjacent tiles
[System.Serializable]
public class Room {

	public int roomID;
	private DungeonCreator dungeonScript;

	//start coordinate isnt always right!
	public Coordinate start;//{ get; private set; }
	public Coordinate size;//{ get; private set; }
	public Coordinate end{ get{return Coordinate.Add(start, size);} }//lazyness

	public List<Tile> adjacentTiles = new List<Tile> ();
	public Tile[,] tiles;

	public bool isComposite;

	public Room(DungeonCreator dungeonScript, Coordinate start, Coordinate size){
		this.dungeonScript = dungeonScript;
		this.start = start;
		this.size = size;
		//end = Coordinate.Add (start, size);

		//select adjacent tiles:
		Debug.DrawLine (dungeonScript.TilePositionFromCoordinate (dungeonScript.dungeonBuildStart), dungeonScript.TilePositionFromCoordinate (dungeonScript.dungeonSize), Color.yellow, 0.5f);
		for(int x = start.x - 1; x < end.x + 1; x ++){
			for(int y = start.y - 1; y < end.y + 1; y ++){
				//adjacent tiles only inside border:
				if(x >= dungeonScript.dungeonBuildStart.x && x <= (dungeonScript.dungeonSize.x - 2) && y >= dungeonScript.dungeonBuildStart.y && y <= (dungeonScript.dungeonSize.y - 2)){
					//make sure adjacent tiles are not within our 'body':
					if (x == (start.x - 1) || x == end.x || y == (start.y - 1) || y == end.y) {
						adjacentTiles.Add (dungeonScript.tiles [x, y]);
					}
				}
			}
		}
	}

	public void TryMergeWithRoom(Room other){
		//kijk of de tiles zelf overlappen:
		if(Overlap(other)){
			isComposite = true;
		}

		//kijk of de adjacent tiles overlappen:
		CheckOverlappedAdjacents(other);

		if (isComposite) {
			other.CheckOverlappedAdjacents (this);
		}
	}

	public void CheckOverlappedAdjacents(Room other){
		//if one of the other' tiles contain my adjacent tile, remove it in 'CheckAdjacentOverlap'
		dungeonScript.PerformActionForArea (other.start, other.size, CheckAdjacentOverlap);		
	}

	private bool Overlap(Room other){
		//check for each corner of the new room whether it overlaps with this room:
		if(CoIntOverlap(start.x, end.x, other.start.x, other.end.x) && 
			CoIntOverlap(start.y, end.y, other.start.y, other.end.y)){
			return true;
		}

		return false;
	}

	private bool CoIntOverlap(int min, int max, int min2, int max2){
		if (min2 >= min && min2 < max)
			return true;

		if (max2 >= min && max2 < max)
			return true;

		return false;
	}

	void CheckAdjacentOverlap(Tile otherRoomTile){
		for(int i = 0; i < adjacentTiles.Count; i ++){
			//zo ja verwijder adjacent tile en maak room composite:
			Tile currentTile = adjacentTiles [i];
			if (currentTile == otherRoomTile) {
				isComposite = true;
				adjacentTiles.Remove (currentTile);
				Debug.DrawRay (dungeonScript.TilePositionFromCoordinate (currentTile.coordinate), Vector3.up, Color.red, 0.5f);
			}
			else {
				Debug.DrawRay (dungeonScript.TilePositionFromCoordinate (currentTile.coordinate), Vector3.up, Color.green, 0.5f);				
			}
		}
	}
}
