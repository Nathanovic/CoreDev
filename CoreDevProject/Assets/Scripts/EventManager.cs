using UnityEngine;
using System.Collections;

//will I even use this?
public class EventManager : MonoBehaviour {

	private static EventManager instance;
	public static EventManager Instance{
		get{ 
			if(instance == null){
				GameObject em = new GameObject ("Event Manager");
				em.AddComponent <EventManager> ();
			}
			return instance;
		}
		set{ 
			instance = value;
		} 
	}

//	public delegate void EventsHandler();
//	public event EventsHandler gameStarted;
//	public event EventsHandler gameQuited;
//
//	void Awake(){
//		Instance = this;
//	}
//
//	public void StartGame(){
//		if (gameStarted != null) {
//			gameStarted.Invoke ();
//		}
//	}
//
//	public void QuitGame(){
//		if(gameQuited != null){
//			gameQuited.Invoke ();
//		}
//	}
}
