using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace AI_UtilitySystem{ 

	public abstract class State : ScriptableObject, IComparable<State> {

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

		public abstract void Run ();

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
			utilityValue = 0f;
			if (decisionFactors.Length > 0) {
				utilityValue = decisionFactors [0].Value (statsModel);

				for (int i = 1; i < decisionFactors.Length; i++) {
					utilityValue *= decisionFactors [i].Value (statsModel);
				}
			}

			if(utilityValue == 0f) {
				utilityValue = 0.001f;
			}

			utilityValue *= OveralUtilityFactor();
		}
			
		public int CompareTo (State other) {
			int otherUtility = (int)other.utility;
			int myUtility = (int)utility;

			if (otherUtility == myUtility)
				return 0;

			return (otherUtility > myUtility) ? 1 : -1;
		}

		public float OveralUtilityFactor(){
			return (float)utility / 3f;
		}
		#endregion
	}
		
	public enum Utility{
		low = 1,
		average = 2,
		high = 3
	}

	[CreateAssetMenu(fileName = "Idle", menuName = "Utility System/State: Idle", order = 1)]
	public class Idle : State{

		[Header("Idle for a random period between:")]
		public float minIdleDuration;
		public float maxIdleDuration;
		private float idleDuration;
		private float passedIdleTime;

		public override void EnterState () {
			idleDuration = UnityEngine.Random.Range (minIdleDuration, maxIdleDuration);
			base.EnterState ();
		}

		public override void Run () {
			passedIdleTime += Time.deltaTime;
			if (passedIdleTime > idleDuration) {
				EndState ();
				return;
			}
		}
	}

	[CreateAssetMenu(fileName = "Charge Forward", menuName = "Utility System/State: Charge fwd", order = 1)]
	public class Charge : State{

		[Header("Charge forward:")]
		public int chargeDamage = 1;

		public LayerMask obstacleLM;
		public float stopForObstacleDist = 0.5f;

		public float maxChargeDist = 10f;
		private float passedChargeDist = 0f;
		public float chargeSpeed = 5f;

		public override void EnterState () {
			passedChargeDist = 0f;
			statsModel.onTriggerOther += TriggerOther;
			if (RaycastObstacle ()) {
				statsModel.forward *= -1;
			}

			base.EnterState ();
		}

		public override void Run () {
			if (RaycastObstacle()) {
				EndState ();
				return;
			}

			//charge forward:
			controller.MoveForward(chargeSpeed);
			passedChargeDist += chargeSpeed * Time.deltaTime;
		}

		private bool RaycastObstacle(){
			if (Physics2D.Raycast (statsModel.position, statsModel.forward, stopForObstacleDist)) {
				return true;
			}
			return false;
		}

		private void TriggerOther(Collider2D other){
			IAttackable target = other.GetComponent<IAttackable> ();
			if (target == statsModel.target) {
				target.ApplyDamage (chargeDamage);
				EndState ();
			}
		}

		protected override void EndState () {
			statsModel.onTriggerOther -= TriggerOther;
			base.EndState ();
		}
	}
}
