using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace AI_UtilitySystem{ 
	//this State is the base for all states ran by the UtilityStateMachine
	public class State : ScriptableObject {

		protected AIBase controller;
		protected AIStats statsModel;
		[SerializeField]private DecisionFactor[] decisionFactors;//these should all be valid in order to transition to me

		public Utility utility;

		public float cooldownTime = 0.5f;
		private UtilityStateMachine stateMachine;

		public bool run{ get; private set; }
		public bool isActive{ get; private set; }
		public float utilityValue{ get; private set; }//should always be normalized!

		//only the state behaviour can be overriden by other states
		#region state behaviour:
		public void Init(UtilityStateMachine sm, AIBase baseScript, AIStats statScript){
			stateMachine = sm;
			statsModel = statScript;
			controller = baseScript;
		}

		public virtual void EnterState(){
			run = true;
			isActive = true;
		}

		public virtual void Run (){}

		protected virtual void EndState(){
			run = false;	
			stateMachine.StartCoroutine (WaitForCountdown ());
		}

		private IEnumerator WaitForCountdown(){
			yield return new WaitForSeconds (cooldownTime);
			isActive = false;
			stateMachine.TriggerNextState ();		
		}
		#endregion

		#region utility behaviour:
		public void CalculateUtility(){
			if (decisionFactors.Length > 0) {
				utilityValue = decisionFactors [0].Value (statsModel);

				for (int i = 1; i < decisionFactors.Length; i++) {
					utilityValue *= decisionFactors [i].Value (statsModel);
				}

				utilityValue *= OveralUtilityFactor ();
			}
			else {
				utilityValue = 0f;
			}
		}

		public float OveralUtilityFactor(){
			return (float)utility / 3f;
		}
		#endregion

		//a copy of the actual State is required to prevent multiple agents from using and changing the same variables
		public virtual State GetCopy (){
			Debug.LogWarning ("not-inherited-warning");
			return null;
		}

		//can be used to set the base-variables for the copy
		protected void SetCopyBase<T>(ref T copyInstance, string instanceName) where T : State{
			copyInstance.name = instanceName;
			copyInstance.decisionFactors = decisionFactors;
			copyInstance.utility = utility;
			copyInstance.cooldownTime = cooldownTime;
		}
	}
		
	public enum Utility{
		low = 1,
		average = 2,
		high = 3
	}
}
