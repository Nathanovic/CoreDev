using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatHandler))]
public class Player : MonoBehaviour {
	
	private Rigidbody rb;
	private Vector3 velocity;
	[SerializeField]private float sprintSpeed;
	[SerializeField]private float horizontalSlowness;
	[SerializeField]private float forwardSpeed;

	[SerializeField]private Hammer hammerScript;

	[SerializeField]private GameObject trailPrefab;
	[SerializeField]private float trailCountdown = 2f;
	private float remainingCountdown;

	[SerializeField]private HealthBar healthBar;

	private void Awake(){
		CombatHandler combatScript = GetComponent<CombatHandler> ();
		combatScript.onFighterDied += GameManager.instance.PlayerDied;
		CombatManager.Instance.InitializeFighter (combatScript, healthBar);
	}

	private void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	private void Update(){
		float v = Input.GetAxisRaw ("Vertical");
		float h = Input.GetAxisRaw ("Horizontal");

		Vector3 inputVector = new Vector3 (h, 0f, v);
		float speed = 0f;
		if (inputVector != Vector3.zero) {
			speed = Input.GetKey (KeyCode.LeftShift) ? sprintSpeed : forwardSpeed;
			speed -= Mathf.Abs(inputVector.x) * horizontalSlowness;
		}

		velocity = new Vector3 (h, 0f, v).normalized * speed;

		if (Input.GetMouseButtonUp (0)) {
			hammerScript.Hit ();
		}

		if (remainingCountdown > 0f) {
			remainingCountdown -= Time.deltaTime;
		}
		else {
			if (Input.GetKeyUp (KeyCode.Space)) {
				Vector3 trailPos = transform.position + transform.forward;
				GameObject.Instantiate (trailPrefab, trailPos, transform.rotation);
				remainingCountdown = trailCountdown;
			}
		}
	}

	private void FixedUpdate(){
		Vector3 relativeVelocity = transform.TransformDirection (velocity);
		rb.MovePosition (rb.position + relativeVelocity * Time.fixedDeltaTime);
	}
}
