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
	private SpriteRenderer sprRenderer;

	public AIBase stats = new AIBase();

	public Transform[] patrolPoints = new Transform[2];
	private int patrolTargetID = 0;
	private Vector3 patrolTarget;
	private Vector3 movement;

	void Start () {
		anim = GetComponent<Animator> ();
		sprRenderer = GetComponent<SpriteRenderer> ();

		SetNewPatrolTarget ();
		InitFSM ();
	}

	void InitFSM(){
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
		eventScript.StartPatrolling ();
	}
	public void Patrol(){
		Debug.Log ("Patrol");
		if (Mathf.Abs (patrolTarget.x - transform.position.x) < 0.1f) {
			SetNewPatrolTarget ();
		}

		transform.Translate (movement * Time.deltaTime);
	}
	void SetNewPatrolTarget(){
		if (patrolTargetID == (patrolPoints.Length - 1)) {
			patrolTargetID = 0;
		} else {
			patrolTargetID++;
		}

		patrolTarget = patrolPoints [patrolTargetID].position;
		float moveRight = (patrolTarget.x > transform.position.x) ? 1f : -1f;
		movement = new Vector2 (moveRight * stats.moveSpeed, 0f);
		SetFacingDirection (patrolTarget);
	}

	public void PrepareAttack(IDamageable target){
		anim.SetFloat ("moveSpeed", 0f);
		target.onDie += eventScript.TargetDies;
	}
	public void Attack(IDamageable target){
		Debug.Log ("Attack " + target.ToString());
		stats.attackTimer += Time.deltaTime;

		if (stats.attackTimer >= stats.attackCountdown) {
			stats.attackTimer = 0f;
			anim.SetBool ("slash", true);
			target.ApplyDamage (stats.attackDamage);
		} else {
			anim.SetBool ("slash", false);
		}
	}
	public void StopAttack(IDamageable target){
		anim.SetBool ("slash", false);
		target.onDie -= eventScript.TargetDies;
	}

	void SetFacingDirection(Vector3 targetPos){
		if (targetPos.x > transform.position.x) {
			sprRenderer.flipX = false;
		} else {
			sprRenderer.flipX = true;
		}
	}
}

[System.Serializable]
public class AIBase{
	public int moveSpeed = 1;
	public int attackDamage = 1;
	public float attackCountdown = 1f;
	public float attackTimer = 0f;
}