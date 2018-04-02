using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI_UtilitySystem{
	//contains all of the stats of a utility-system-based agent
	//does all the relevant sensing for the agent's surroundings
	[System.Serializable]
	public class AIStats : MonoBehaviour{

		public CombatHandler fighter;
		public CombatHandler target;

		public float weaponRange = 1.2f;

		public NavMeshAgent nmAgent;

		private void Start(){
			target = GameObject.FindGameObjectWithTag ("Player").GetComponent<CombatHandler> ();
			nmAgent = GetComponent<NavMeshAgent> ();
			nmAgent.stoppingDistance = weaponRange;
		}

		private float DistToTarget(){
			if (target != null) {
				return SimpleDistToTarget ();
			}

			return Mathf.Infinity;
		}

		public float SimpleDistToTarget(){
			return Vector3.Distance (target.Position (), transform.position);
		}

		public float DistToDestination(){
			return Vector3.Distance (nmAgent.destination, transform.position);
		}

		public bool ValidPathAvailable(Vector3 targetDestination){
			Vector3 currentDestination = nmAgent.destination;
			nmAgent.SetDestination (targetDestination);
			if (nmAgent.pathStatus == NavMeshPathStatus.PathComplete) {
				return true;
			}
			else {
				nmAgent.SetDestination (currentDestination);
				return false;
			}
		}

		public float GetStatValue(Stat s){
			switch (s) {
			case Stat.DistToTarget:
				return DistToTarget ();
			case Stat.Health:
				return fighter.hitPoints;
			default:
				Debug.LogWarning ("Unknown stat: " + s.ToString ());
				return 0f;
			}
		}
	}

	public enum Stat{
		DistToTarget,
		Health
	}
}