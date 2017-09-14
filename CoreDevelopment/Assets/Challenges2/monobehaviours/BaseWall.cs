using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment2{
	public abstract class BaseWall : MonoBehaviour, IDamageable {
		
		#region IDamageable implementation
		public void TakeDamage (int dmg)
		{
			throw new System.NotImplementedException ();
		}
		public void Die ()
		{
			throw new System.NotImplementedException ();
		}

		private int health;
		public int Health {
			get{ 
				return health;
			}
			set{ 
				if (value <= 0)
					health = 0;
				else
					health = value;
			}
		}
		#endregion
	}
}