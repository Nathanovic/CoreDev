using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Messenger.SendMessage("OnHitEvent") -> stuurt naar iedereen
/// Messenger.SendMessage("OnHitEvent", gameObject) -> stuurt naar gameObject
/// </summary>

public static class Messenger {

	private static Dictionary<string, Dictionary<GameObject, Action>> dictionary = new Dictionary<string, Dictionary<GameObject, Action>> ();

	public static void SendMessage(string messageName, params GameObject[] objs){
		if (!dictionary.ContainsKey (messageName)) {
			Debug.Log (messageName + " is an invalid message!");
			return;
		}

		if (objs.Length > 0) {
			foreach (GameObject obj in objs) {
				if (dictionary [messageName].ContainsKey (obj)) {
						
				}
			}
		} 
		else {
			//foreach(KeyValuePair<string, GameObject>() in dictionary){
				
			//}
		}
	}

	public static void AddListener(string messageName, GameObject gameObject, Action action){
		if (dictionary.ContainsKey (messageName)) {
			if (dictionary [messageName].ContainsKey (gameObject)) {
				dictionary [messageName] [gameObject] += action;
			} 
			else {
				dictionary [messageName].Add (gameObject, action);
			}
		} 
		else {
			dictionary.Add (messageName, new Dictionary<GameObject, Action>());	
			AddListener (messageName, gameObject, action);
		}
	}

	public static void RemoveListener(string messageName, GameObject gameObject, Action action){
		if (dictionary.ContainsKey (messageName)) {
			if (dictionary [messageName].ContainsKey (gameObject)) {
				dictionary [messageName] [gameObject] -= action;
			} 
		} 
	}
}
