using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomOverlapTester : MonoBehaviour {

	public Room firstRoom;
	public Room secondRoom;

	void Update(){
		if(Input.GetKeyUp(KeyCode.A)){
			//Debug.Log ("overlap: " + firstRoom.Overlap (secondRoom).ToString());
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		
		Vector3 size = new Vector3 (firstRoom.size.x, firstRoom.size.y);
		Vector3 start = new Vector3 (firstRoom.start.x, firstRoom.start.y) + size / 2f;
		Gizmos.DrawWireCube (start, size);

		size = new Vector3 (secondRoom.size.x, secondRoom.size.y);
		start = new Vector3 (secondRoom.start.x, secondRoom.start.y) + size / 2f;
		Gizmos.DrawWireCube (start, size);
	}
}
