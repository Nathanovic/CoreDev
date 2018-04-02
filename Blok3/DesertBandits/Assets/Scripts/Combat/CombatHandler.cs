using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HealthChangedDelegate(int newHealth);

public class CombatHandler : MonoBehaviour, IAttackable {

	public int maxHitPoints;
	public int hitPoints{ get; private set; }
	public event HealthChangedDelegate onHealthChanged;
	public bool initialized;

	public delegate void SimpleDelegate();
	public event SimpleDelegate onFighterDied;

	void Start(){
		hitPoints = maxHitPoints;

		if (!initialized)
			Debug.LogWarning (transform.name + " has not yet been initialized!");
	}

	public void ApplyDamage (int dmg) {
		hitPoints -= dmg;
		if (hitPoints <= 0) {
			hitPoints = 0;
			if (onFighterDied != null) {
				onFighterDied ();
			}
		}

		if (onHealthChanged != null) {
			onHealthChanged (hitPoints);
		}
	}

	public bool ValidTarget (){
		return hitPoints > 0;
	}

	public Vector3 Position () {
		return transform.position;
	}
}
