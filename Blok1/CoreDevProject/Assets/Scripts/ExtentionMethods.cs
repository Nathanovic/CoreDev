using UnityEngine;
using System.Collections;

public static class ExtentionMethods {

	public static Vector2 GetPosition(this Transform transform){
		return new Vector2 (transform.position.x, transform.position.y);
	}

	public static void SetPosition(this Transform transform, Vector2 myPos){
		transform.position = new Vector3 (myPos.x, myPos.y);
	}
}
