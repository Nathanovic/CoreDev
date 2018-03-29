using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this script generates a random map using the cellular automata rules
public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;

	public int[,] map;//0 === empty, 1 === wall

	public int smoothValue = 5;

	void Start(){
		GenerateMap ();
	}

	void Update(){
		if (Input.GetKeyUp (KeyCode.Space)) {
			GenerateMap ();
		}
	}

	void GenerateMap(){
		map = new int[width, height];
		RandomFillMap ();

		for (int i = 0; i < smoothValue; i++) {
			DoActionForAllTiles (SmoothTile);
		}

		//add a border to the map:
		int borderSize = 5;
		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength (0); x++) {
			for (int y = 0; y < borderedMap.GetLength (1); y++) {
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)//x inside of the map(it's not a border) 
					borderedMap [x, y] = map [x - borderSize, y - borderSize];
				else
					borderedMap [x, y] = 1;
			}
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator> ();
		meshGen.GenerateMesh (borderedMap, 1f);
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
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
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

	/*
	void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, 1f));

		if (map == null)
			return;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
				Vector3 pos = TilePos (x, y);
				Gizmos.DrawCube (pos, Vector3.one * 0.95f);
			}
		}
	}
	*/

	Vector3 TilePos(int x, int y){
		return new Vector3 (-(float)width / 2f + x + .5f, -(float)height / 2f + y + .5f);
	}

	void DoActionForAllTiles(UnityAction<int, int> tileCallback){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tileCallback (x, y);
			}
		}
	}
}
