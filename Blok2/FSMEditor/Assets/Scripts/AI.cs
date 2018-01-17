using UnityEngine;
using System.Collections.Generic;

//base class for all AI
//the fsm is being ran from void Update, the state-changes are being handled inside the conditions
[RequireComponent(typeof(AIEvents))]
public class AI : MonoBehaviour {

	private AIEvents eventScript;

	[SerializeField]private FSMData fsmMaker;
	[SerializeField]private FSM fsm;
	private Animator anim;

	public AIBase stats = new AIBase();

	void Start () {
		anim = GetComponent<Animator> ();

		fsm = new FSM (this);
		eventScript = GetComponent<AIEvents> ();

		foreach (Condition condition in fsmMaker.conditions) {
			condition.Init (fsm, eventScript);
		}

		fsm.UpdateState (fsmMaker.states [0]);
	}

	void Update(){
		fsm.Run ();
	}

	public void PrepareIdle(){
		anim.SetFloat ("moveSpeed", 0f);
	}
	public void Idle(){
		Debug.Log ("Idle");
	}

	public void PreparePatrol(){
		anim.SetFloat ("moveSpeed", 1f);
	}
	public void Patrol(){
		Debug.Log ("Patrol");
	}

	public void PrepareAttack(IDamageable target){
		anim.SetBool ("attack", true);
		anim.SetFloat ("moveSpeed", 0f);
		target.onDie += eventScript.TargetDies;
	}
	public void Attack(IDamageable target){
		Debug.Log ("Attack " + target);
		stats.attackTimer += Time.deltaTime;
		if (stats.attackTimer >= stats.attackCountdown) {
			stats.attackTimer = 0f;
			target.ApplyDamage (stats.attackDamage);
		}
	}
	public void StopAttack(IDamageable target){
		anim.SetBool ("attack", false);
		target.onDie -= eventScript.TargetDies;
	}
}

[System.Serializable]
public class AIBase{
	public int moveSpeed = 1;
	public int attackDamage = 1;
	public float attackCountdown = 1f;
	public float attackTimer = 0f;
}