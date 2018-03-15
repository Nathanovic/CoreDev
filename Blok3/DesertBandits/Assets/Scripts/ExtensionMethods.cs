using UnityEngine;

public static class ExtensionMethods {

	public static Vector3 Position (this Coordinate coord){
		return new Vector3(coord.x, coord.y, 0f);
	}

	public static string StringValue(this Coordinate coord){
		return "[" + coord.x + "," + coord.y + "]";
	}
}
