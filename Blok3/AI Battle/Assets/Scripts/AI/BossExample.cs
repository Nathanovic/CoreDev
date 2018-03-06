using System.Collections.Generic;
using UnityEngine;

public class BossExample : MonoBehaviour {

	private EnemyController hitReceiver;
	private Counter waitForChargeCounter;
	private Rigidbody2D rb;
	[SerializeField]private int health = 6;

	[Header("Move variables:")]
	[SerializeField]private float maxSpeed;
	[SerializeField]private float reachedTargetDist;

	[Header("Charge variables:")]
	[SerializeField]private float chargeWaitTime = 2f;
	[SerializeField]private List<BossCharge> availableCharges = new List<BossCharge>(2);
	private List<BossCharge> unlockedCharges = new List<BossCharge>(2);
	private BossCharge currentCharge;
	private int nextChargeIndex = -1;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();

		//make sure that we can be damaged:
		hitReceiver = GetComponent<EnemyController> ();
		hitReceiver.onHit += ReceiveDamage;

		//set up charges:
		foreach(BossCharge charge in availableCharges){
			charge.Init (this);
		}
		unlockedCharges.Add(availableCharges[0]);
		PrepareNextCharge ();

		//set up a counter between the charges:
		waitForChargeCounter = new Counter (chargeWaitTime);
		waitForChargeCounter.onCount += PrepareNextCharge;
	}

	void Update () {
		if (currentCharge != null) {
			currentCharge.Run ();
		}
	}

	public void Move(Vector3 targetPos, out bool reachedTargetPos, float speedMultiplier = 1f){
		if (Mathf.Abs (targetPos.y - transform.position.y) > 0.2f) {
			transform.position = targetPos;
		}
		else {
			Vector3 dir = targetPos - transform.position;
			Vector3 movement = dir.normalized * maxSpeed * speedMultiplier;
			rb.velocity = new Vector2 (movement.x, movement.y);			
		}

		if (Vector3.Distance (transform.position, targetPos) > reachedTargetDist) {
			reachedTargetPos = false;
		}
		else {
			reachedTargetPos = true;
		}
	}

	void ReceiveDamage(){
		health--;
		if (health == 3) {//start charge 2!
			unlockedCharges.Add (availableCharges [1]);
			nextChargeIndex = 1;
		}
	}

	void PrepareNextCharge(){
		if (nextChargeIndex > -1) {
			currentCharge = unlockedCharges [nextChargeIndex];
			nextChargeIndex = -1;
		}
		else {
			int rndmIndex = Random.Range (0, unlockedCharges.Count);
			currentCharge = unlockedCharges [rndmIndex];
		}

		currentCharge.Activate ();
	}

	public void ChargeStops(){
		currentCharge = null;
		waitForChargeCounter.StartCounting ();
	}

	public float DistFromMe(Vector3 targetPos){
		return Vector3.Distance(transform.position, targetPos);
	}

	public void EnableFlying(bool enableFlying){
		rb.gravityScale = enableFlying ? 0f : 1f;
		rb.velocity = Vector3.zero;
	}
}

[System.Serializable]
public class BossCharge {
	private BossExample boss;
	public ChargeState myState = ChargeState.disabled;

	private Counter feedForwardCounter;
	[SerializeField]private float feedForwardTime = 1f;
	[SerializeField]private float chargeSpeedFactor = 2.5f;

	[SerializeField]private ChargePoints[] chargeSetups;
	private ChargePoints currentCharge;

	private bool fly;

	public void Init(BossExample _boss){
		boss = _boss;

		feedForwardCounter = new Counter (feedForwardTime);
		feedForwardCounter.onCount += StartCharge;

		for (int i = 0; i < chargeSetups.Length; i++) {
			chargeSetups [i].Init ();
		}
	}

	public virtual void Activate(){
		myState = ChargeState.prepare;

		float closestDist = 1000f;
		int closestSetupIndex = 0;
		for (int i = 0; i < chargeSetups.Length; i++) {
			float dist = boss.DistFromMe (chargeSetups [i].start);
			if (dist < closestDist) {
				closestDist = dist;
				closestSetupIndex = i;
			}
		}
		currentCharge = chargeSetups[closestSetupIndex];

		if (currentCharge.end.x == currentCharge.start.x) {
			boss.EnableFlying (true);
			fly = true;
			UnityEditor.EditorApplication.isPaused = true;
		}
		else {
			fly = false;
		}
	}

	public void Run(){
		switch (myState) {
		case ChargeState.prepare:
			MoveToStartPos ();
			break;
		case ChargeState.charge:
			ChargeToEndPos ();
			break;
		}
	}

	void MoveToStartPos(){
		bool reachedPos = false;
		boss.Move (currentCharge.start, out reachedPos);
		if (reachedPos) {
			myState = ChargeState.feedForward;
			feedForwardCounter.StartCounting ();
		}
	}

	void StartCharge(){
		boss.EnableFlying (false);
		myState = ChargeState.charge;
	}

	void ChargeToEndPos(){
		bool reachedPos = false;
		boss.Move (currentCharge.end, out reachedPos, chargeSpeedFactor);
		if (reachedPos) {
			StopCharging ();
		}
	}

	protected virtual void StopCharging (){
		myState = ChargeState.disabled;
		boss.ChargeStops ();
	}
}

[System.Serializable]
public class ChargePoints{
	[SerializeField]private Transform startTransform;
	[SerializeField]private Transform endTransform;
	[HideInInspector]public Vector3 start;
	[HideInInspector]public Vector3 end;

	public void Init(){
		start = startTransform.position;
		end = endTransform.position;
	}
}

public enum ChargeState{
	prepare,		//move to the right position
	feedForward,	//show that we will do something
	charge,			//charge towards the player!
	disabled		//do nothing
}