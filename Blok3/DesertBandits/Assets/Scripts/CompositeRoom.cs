using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompositeRoom {

	public List<Room> connectedRooms;

	public CompositeRoom(List<Room> rooms){
		connectedRooms = new List<Room> ();
		AddRooms (rooms);
	}

	public void AddRooms(List<Room> rooms){
		connectedRooms.AddRange (rooms);
		foreach (Room r in rooms) {
			r.isComposite = true;
		}
	}

	public void MergeComposite(CompositeRoom other){
		AddRooms (other.connectedRooms);
	}

	public bool Overlap (CompositeRoom other){
		foreach (Room r in other.connectedRooms) {
			if (Overlap (r)) {
				return true;
			}
		}

		return false;
	}

	public bool Overlap (Room other){
		foreach (Room r in connectedRooms) {
			if (r.Overlap (other)) {
				return true;
			}
		}

		return false;
	}
}
