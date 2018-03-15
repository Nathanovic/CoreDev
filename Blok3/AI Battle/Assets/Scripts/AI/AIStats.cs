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
		public Vector2 forward;

		public int maxHealth = 10;
		public int health = 10;
		public int maxEnergy;
		public int energy;

		public IAttackable target;

		private LayerMask obstacleLM;
		public float stopForObstacleDist = 0.1f;

		public float weaponRange = 1.2f;

		private void Start(){
			obstacleLM = LayerMask.GetMask ("Default", "Ground", "Obstacle");

			health = maxHealth;
			energy = maxEnergy;

			forward = new Vector2 (1, 0);
		}

		private float DistToTarget(){
			if (target == null)
				return Mathf.Infinity;
			else
				return Vector3.Distance (transform.position, target.Position());
		}
			
		public bool ObstacleAhead(){
			return ObstacleAhead (obstacleLM, stopForObstacleDist);
		}

		public bool ObstacleAhead(LayerMask objectLM, float distance){
			Debug.DrawRay (position, forward * distance, Color.red);
			if (Physics2D.Raycast (position, forward, distance, objectLM)) {
				return true;
			}
			return false;
		}

		//used to determine whether we are stuck or not
		private float FarthestObjectDist(){
			float farthestDist = 30f;
			RaycastHit2D hitRight = Physics2D.Raycast (position, Vector2.right, 30f);
			RaycastHit2D hitLeft = Physics2D.Raycast (position, -Vector2.right, 30f);

			if (hitRight.collider != null) {
				farthestDist = hitRight.distance;	
			}
			if (hitLeft.collider != null) {
				farthestDist = Mathf.Max (farthestDist, hitLeft.distance);
			}

			return farthestDist;
		}

		public float GetStatValue(Stat s){
			switch (s) {
			case Stat.DistToTarget:
				return DistToTarget ();
			case Stat.Energy:
				return energy;
			case Stat.Health:
				return health;
			case Stat.FarthestObjectDist:
				return FarthestObjectDist ();
			default:
				Debug.LogWarning ("Unknown stat: " + s.ToString ());
				return 0f;
			}
		}
	}

	public enum Stat{
		DistToTarget,
		Health,
		Energy,
		FarthestObjectDist
	}
}