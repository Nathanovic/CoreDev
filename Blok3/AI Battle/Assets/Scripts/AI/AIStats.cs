using System.Collections.Generic;
using UnityEngine;

namespace AI_UtilitySystem{
	//contains all of the stats of a utility-system-based agent
	//does all the relevant sensing for the agent's surroundings
	[System.Serializable]
	public class AIStats : MonoBehaviour{

		private Vector2 position{
			get{ 
				return new Vector2 (transform.position.x, transform.position.y);
			}
		}
		public Vector2 forward{ get; set;}

		public int maxHealth = 10;
		public int health = 10;
		public int maxEnergy;
		public int energy;

		public IAttackable target;

		private LayerMask obstacleLM;
		public float stopForObstacleDist = 0.1f;

		public delegate void TriggerOtherDelegate (Collider2D other);
		public event TriggerOtherDelegate onTriggerOther;

		void Start(){
			obstacleLM = LayerMask.GetMask ("Default", "Ground", "Obstacle");

			health = maxHealth;
			energy = maxEnergy;

			forward = new Vector2 (1, 0);
		}

		public float DistToTarget(){
			if (target == null)
				return Mathf.Infinity;
			else
				return Vector3.Distance (transform.position, target.Position());
		}
			
		public bool ObjectAhead(){
			return ObjectAhead (obstacleLM, stopForObstacleDist);
		}

		public bool ObjectAhead(LayerMask objectLM, float distance){
			Debug.DrawRay (position, forward.normalized * distance, Color.red);
			if (Physics2D.Raycast (position, forward, distance, objectLM)) {
				return true;
			}
			return false;
		}

		public float GetStatValue(Stat s){
			switch (s) {
			case Stat.DistToTarget:
				return DistToTarget ();
			case Stat.Energy:
				return energy;
			case Stat.Health:
				return health;
			default:
				Debug.LogWarning ("Unknown stat: " + s.ToString ());
				return 0f;
			}
		}

		private void OnTriggerEnter2D(Collider2D other){
			if (onTriggerOther != null) {
				onTriggerOther (other);
			}
		}
	}

	public enum Stat{
		DistToTarget,
		Health,
		Energy
	}
}