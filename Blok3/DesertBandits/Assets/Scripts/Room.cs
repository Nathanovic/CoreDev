using UnityEngine;

[System.Serializable]
public class Room {

	//start coordinate isnt always right!
	public Coordinate start;//{ get; private set; }
	public Coordinate size{ get; private set; }
	public Coordinate end{ get; private set; }

	public bool isComposite;

	public Room(Coordinate start, Coordinate size){
		this.start = start;
		this.size = size;
		end = Coordinate.Add (start, size);
	}

	public bool Overlap(Room other){
		//check for each corner of the new room whether it overlaps with this room:
		if(CoIntOverlap(start.x - 1, end.x + 1, other.start.x, other.end.x) && 
			CoIntOverlap(start.y - 1, end.y + 1, other.start.y, other.end.y)){
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
}
