using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AI_UtilitySystem;

//this is the script that actually makes our agent do stuff the player sees, this behaviour is entirely controlled from the states
//this is the only script that can change values of the AIStats script
[RequireComponent(typeof(AIStats))]
public class AIBase : MonoBehaviour, IAttackable {

	private Rigidbody2D rb;
	private Animator anim;
	private AIStats myStats;

	private SpriteRenderer myRenderer;
	private Color myColor;

	[SerializeField]private Transform worldCanvas;
	[SerializeField]private HealthBar healthBar;

	public delegate void HealthUpdateDelegate (int newHP);
	public event HealthUpdateDelegate onHealthChanged;

	void Awake(){
		CombatManager.Instance.RegisterPotentialTarget (this);

		rb = GetComponent<Rigidbody2D> ();
		myStats = GetComponent<AIStats> ();

		anim = transform.GetChild(0).GetComponent<Animator> ();

		myRenderer = GetComponentInChildren<SpriteRenderer> ();
		myColor = myRenderer.color;

		myStats.previousPosition = Position ();
	}

	void Start(){
		//spawn the healthbar and initialize it:
		healthBar = GameObject.Instantiate(healthBar, worldCanvas) as HealthBar;
		healthBar.Init (this);
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

		if (bestTarget != null) {
			if ((bestTarget.Position ().x > transform.position.x && myStats.forward.x < 0f) ||
				bestTarget.Position().x < transform.position.x && myStats.forward.x > 0f) {
				FlipFacingDirection ();	
			}
		}

		myStats.target = bestTarget;
	}

	public void ApplyDamage (int dmg) {
		myStats.health -= dmg;
		if (myStats.health <= 0) {
			myStats.health = 0;
		}

		if (onHealthChanged != null) {
			onHealthChanged (myStats.health);
		}
	}

	public bool ValidTarget (){
		return myStats.health > 0;
	}

	//can be called to make sure we are facing towards the target (if there is any)
	//if inverted is true, we probably are trying to get away from the target
	public void TryFacingDanger(bool inverted = false){
		if (myStats.target == null) {
			return;
		}

		Vector2 targetPos = new Vector2 (myStats.target.Position ().x, transform.position.y);
		Vector2 desiredDir = targetPos - Position ();
		desiredDir = inverted ? -desiredDir : desiredDir;
		float dotProduct = Vector2.Dot (myStats.forward, desiredDir);

		if (dotProduct < 0f) {
			FlipFacingDirection ();	
		} else {
			myStats.forward = desiredDir.normalized;
		}
	}

	public void SetAnimBool(string name, bool value){
		anim.SetBool (name, value);
	}

	public void SetTrigger(string name){
		anim.SetTrigger (name);
	}

	public Vector2 Position(){
		return new Vector2 (transform.position.x, transform.position.y);
	}

	public Vector2 PrevPosition(){
		return myStats.previousPosition;
	}

	public void MoveForward(float speed){
		rb.velocity = new Vector2(myStats.forward.x * speed, rb.velocity.y);
	}

	public void SetForward(Vector2 newForward){
		myStats.forward = newForward;
	}

	public void MoveUp(float speed){
		rb.velocity = Vector2.up * speed;
	}

	public void SetPreviousPosition(){//used to calculate the EnemyApproachSpeed (as a condition factor)
		myStats.previousPosition = rb.position;		
	}

	public void FlipFacingDirection(){
		float newXScale = (myStats.forward.x > 0f) ? -1f : 1f;
		myStats.forward = new Vector2 (newXScale, 0f);
		transform.localScale = new Vector3 (newXScale, 1f, 1f);
	}

	public void FadeSelfOut(float newAlpha){
		myColor.a = newAlpha;
		myRenderer.color = myColor;
	}

	public void DestroySelf(){
		Destroy (gameObject);
		CombatManager.Instance.PotentialTargetDied (this);
	}
}
