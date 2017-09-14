using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable {

	private int health;

	private void Die(){
		//kill me
		Destroy(gameObject);
	}	

	#region IDamageable implementation
	public void OnDeath ()
	{
		Die ();
	}

	public bool TakeDamage (IDamager damager)
	{
		if (damager.Damage > 0) {
			return true;
		}
		return false;
	}
	#endregion
}
