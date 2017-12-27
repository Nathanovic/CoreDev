using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	
	public string buildingName;
	public int cost;
	public Material material;

	public void ResizeMe(float mySize){
		transform.localScale = Vector2.one * mySize;
	}
}

[System.Serializable] 
public enum Material{
	wood,
	brick,
	concrete
}
