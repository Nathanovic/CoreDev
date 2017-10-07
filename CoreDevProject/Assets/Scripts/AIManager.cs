using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

	private static AIManager instance;
	public static AIManager Instance{
		get{ 
			if (instance == null) {
				instance = new GameObject ("EnemyManager").AddComponent<AIManager>();
			}
			return instance;
		}
		set{ 
			instance = value;
		}
	}

	[Header("Debug only:")]
	public List<AI> myAIs;

	private void Awake(){
		Instance = this;	
		myAIs = new List<AI> (10);
	}

	private void Update(){
		foreach(AI myAI in myAIs){
			myAI.UpdateFSM ();
		}
	}

	public void SubscribeAI(AI newAI){
		myAIs.Add (newAI);
		newAI.onDeath += MyAIDies;
	}
	//cant be called by OnDisable yet, so it's private
	private void RemoveAI(AI disabledAI){
		myAIs.Remove (disabledAI);
		disabledAI.onDeath -= MyAIDies;
	}
	
	private void MyAIDies(Character sender){
		AI deadAI = (AI)sender;
		RemoveAI (deadAI);
	}
}
