using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;

public class CharacterMaker : MonoBehaviour {

	public MonoBehaviour c;

	// Use this for initialization
	void Start () {
		Type objectType = c.GetType ();
		Type baseType = objectType.BaseType;
		MethodInfo[] infoArray = objectType.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		List<MethodInfo> info = infoArray.ToList ();
		MethodInfo[] baseInfo = baseType.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		bool[] baseTypeMethods = new bool[baseInfo.Length];
		Debug.Log (info.Count);

		for (int i = 0; i < baseInfo.Length; i++) {
			if (info.Contains (baseInfo [i])) {
				//Debug.Log ("remove: " + baseInfo [i].Name);
				info.Remove (baseInfo [i]);
			} else {
				//Debug.Log ("Unique: " + baseInfo [i].Name);
			}
		}

		foreach (MethodInfo m in info) {
			//Debug.Log ("remaining: " + m.Name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
