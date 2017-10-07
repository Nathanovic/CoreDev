using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment2{
	public interface IDamageable{
		int Health{ get; set;}
		void TakeDamage (int dmg);
		void Die ();
	}
}