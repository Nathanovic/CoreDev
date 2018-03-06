using UnityEngine;
using System.Collections.Generic;
using AI_UtilitySystem;

//this is the only script that can change values of the AIStats script
[RequireComponent(typeof(AIStats))]
public class AIBase : MonoBehaviour, IAttackable {

	private AIStats myStats;

	void Awake(){
		CombatManager.Instance.RegisterPotentialTarget (this);
		myStats = GetComponent<AIStats> ();
	}
		
	//make sure that we have the most potential target set
	public void UpdateTarget(){
		IAttackable bestTarget = null;
		float closestDist = 100000f;

		foreach (IAttackable attackable in CombatManager.Instance.potentialTargets) {
			if (attackable == (IAttackable)this)
				continue;

			float dist = Vector2.Distance (attackable.Position (), Position ());
			if (dist < closestDist && attackable.ValidTarget()) {
				closestDist = dist;
				bestTarget = attackable;
			}
		}

		myStats.target = bestTarget;
	}

	public void ApplyDamage (int dmg) {
		myStats.health -= dmg;
		if (myStats.health <= 0) {
			myStats.health = 0;
		}
	}

	public bool ValidTarget (){
		return myStats.health > 0;
	}

	public Vector2 Position(){
		return new Vector2 (transform.position.x, transform.position.y);
	}

	public void MoveForward(float speed){
		transform.Translate (myStats.forward * speed * Time.deltaTime);
	}
}
