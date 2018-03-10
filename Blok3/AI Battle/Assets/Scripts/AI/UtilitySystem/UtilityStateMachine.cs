using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_UtilitySystem{
	//makes sure that one state is being ran all the time
	[RequireComponent(typeof(AIStats))]
	public class UtilityStateMachine : MonoBehaviour {

		private AIStats statsModel;
		private AIBase controller;
		public List<State> allStates;

		public State[] states;
		public DecisionFactor decFactor;

		public State currentState;

		void Start(){
			statsModel = GetComponent<AIStats> ();
			controller = GetComponent<AIBase> ();

			List<State> copyStatesList = new List<State> (allStates.Count);
			foreach (State s in allStates) {
				State copyInstance = s.GetCopy ();
				copyInstance.Init (this, controller, statsModel);
				copyStatesList.Add (copyInstance);
			}

			allStates = copyStatesList;

			//allStates.Sort ();//pretty useless

			TriggerNextState ();
		}

		//check what state to perform and transition to that state:
		public void TriggerNextState(){
			controller.UpdateTarget ();

			float hightestUtilityValue = 0f;
			State highestUtilityState = null;

			foreach (State state in allStates) {
				state.CalculateUtility ();
				if (state.utilityValue > hightestUtilityValue) {
					hightestUtilityValue = state.utilityValue;
					highestUtilityState = state;
				}
			}

			Debug.Log ("transition to: " + highestUtilityState.name);
			currentState = highestUtilityState;
			currentState.EnterState ();
		}

		void Update(){
			if (currentState.run) {
				currentState.Run ();
			}
		}
	}
}
