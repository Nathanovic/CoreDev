using System.Collections.Generic;
using UnityEngine;

public class CountManager : MonoBehaviour {

	private static CountManager _instance;
	public static CountManager Instance{ 
		get{
			if (_instance == null) {
				GameObject go = new GameObject ("CountManager");
				_instance = go.AddComponent<CountManager> ();
			}

			return _instance;
		} 
	}

	private List<Counter> myCounters;

	void Awake () {
		myCounters = new List<Counter> ();
	}

	public void InitCounter(Counter c){
		myCounters.Add (c);
	}

	void Update () {
		for (int i = 0; i < myCounters.Count; i++) {
			myCounters [i].Count ();
		}
	}
}