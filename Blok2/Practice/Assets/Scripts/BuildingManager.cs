using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour {

	[HideInInspector]public float bridgeSize;//editor
	[HideInInspector]public float houseSize;//editor

	public BuildingType buildingType = BuildingType.house;

	public List<Bridge> bridges = new List<Bridge>();
	public List<House> houses = new List<House>();

	public void AddBridge(Bridge b){
		bridges.Add (b);
	} 

	public void AddHouse(House h){
		houses.Add (h);
	}

	public void ResizeBuildings(){//editor
		if (buildingType == BuildingType.bridge) {
			foreach (Bridge b in bridges)
				b.ResizeMe (bridgeSize);
		} else {
			foreach (House h in houses)
				h.ResizeMe (houseSize);
		}
	}

	public string SizePropertyPath(){
		return (buildingType == BuildingType.bridge) ? "bridgeSize" : "houseSize";
	}
}

[System.Serializable]
public enum BuildingType{
	house,
	bridge
}