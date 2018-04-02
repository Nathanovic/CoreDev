using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AI_UtilitySystem;

//this is the script that actually makes our agent do stuff the player sees, this behaviour is entirely controlled from the states
//this is the only script that can change values of the AIStats script
[RequireComponent(typeof(AIStats), typeof(CombatHandler))]
public class AIBase : MonoBehaviour {

	//private Rigidbody2D rb;
	private Animator anim;
	private AIStats myStats;

	private Material renderMaterial;
	private Color myColor;

	void Awake(){
		myStats = GetComponent<AIStats> ();

		anim = transform.GetChild(0).GetComponent<Animator> ();

		renderMaterial = GetComponent<Renderer> ().material;
		myColor = renderMaterial.color;

		myStats.fighter = GetComponent<CombatHandler> ();
		myStats.fighter.onFighterDied += GameManager.instance.EnemyDied;
		CombatManager.Instance.InitializeFighter (myStats.fighter);
	}
		
	//make sure that we have the best potential target set if there is one
	public void UpdateTarget(){
		CombatHandler bestTarget = null;
		float closestDist = 100000f;

		foreach (CombatHandler attackable in CombatManager.Instance.potentialTargets) {
			if (attackable == myStats.fighter)
				continue;

			float dist = Vector3.Distance (attackable.Position (), Position ());
			if (dist < closestDist && attackable.ValidTarget() && myStats.ValidPathAvailable (attackable.transform.position)) {
				closestDist = dist;
				bestTarget = attackable;
			}
		}

		myStats.target = bestTarget;
	}

	public void SetAnimBool(string name, bool value){
		anim.SetBool (name, value);
	}

	public void UpdateDestination(Vector3 destination){
		myStats.nmAgent.SetDestination (destination);
	}

	public void UpdateTargetDestination(){
		myStats.nmAgent.SetDestination (myStats.target.Position ());
	}

	//position for simple y calculations
	public Vector3 Position(){
		return new Vector3 (transform.position.x, 0, transform.position.z);
	}

	public void SetAgentSpeed (float moveSpeed){
		myStats.nmAgent.speed = moveSpeed;
	}

	public void FadeSelfOut(float newAlpha){
		myColor.a = newAlpha;
		renderMaterial.SetColor("_Color", myColor);
	}

	public void DestroySelf(){
		Destroy (gameObject);
		CombatManager.Instance.FighterDied (myStats.fighter);
	}
}
