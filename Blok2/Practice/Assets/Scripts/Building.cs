using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public string buildingName;
	public int cost;
	public Material material;
}

[System.Serializable] 
public enum Material{
	wood,
	brick,
	concrete
}
