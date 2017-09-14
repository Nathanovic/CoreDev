using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment2{
	public abstract class BaseEnemy : MonoBehaviour, IDamageable {

		public EnemyStats stats;
		public List<Transform> targetLocations = new List<Transform> ();
		public Transform targetLocTr;

		public Transform frontRayOrigin;
		public float checkFronRayLength;

		void Init(){
			health = stats.maxHealth;
		}

		protected void MoveMe(){
			if (CanMoveForward ()) {
				transform.Translate (transform.up * stats.moveSpeed * Time.deltaTime);
			}
		}

		protected void RotateMe(){
			Vector3 targetDir = targetLocTr.position - transform.position;
			Quaternion targetRot = Quaternion.LookRotation (targetDir, transform.forward);
			transform.rotation = targetRot;//should lerp rot using Quaternion....
		}

		protected void DealDamage(IDamageable target){
			target.TakeDamage (stats.attackDamage);
		}

		protected virtual bool CanMoveForward(){
			if(Physics2D.Raycast (frontRayOrigin.position, transform.up, checkFronRayLength)){
				return false;
			}
			else{
				return true;
			}
		}

		#region IDamageable implementation
		public void TakeDamage (int dmg)
		{
			Health -= dmg;
			if (Health == 0)
				Die ();
		}

		public void Die(){
			Destroy (gameObject);
		}

		private int health;
		public int Health {
			get{ 
				return health;
			}
			set{ 
				if (value <= 0)
					health = 0;
				health = value;
			}
		}
		#endregion
	}
}