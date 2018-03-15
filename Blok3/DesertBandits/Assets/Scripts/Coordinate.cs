using UnityEngine;

//coordinates should all be integers, so we need a custom 'vector' for it:
[System.Serializable]
public class Coordinate{
	public int x, y;

	public Coordinate(int x, int y){
		this.x = x;
		this.y = y;
	}

	public static Coordinate Random(Coordinate start, Coordinate end){
		int x = UnityEngine.Random.Range (start.x, end.x);
		int y = UnityEngine.Random.Range (start.y, end.y);
		return new Coordinate(x, y);
	}

	public static Coordinate ZeroCoord(){
		return new Coordinate (0, 0);
	}

	public static Coordinate Add(Coordinate a, Coordinate b){
		int x = a.x + b.x;
		int y = a.y + b.y;
		return new Coordinate (x, y);
	}
}
