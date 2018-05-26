using UnityEngine;
using UnityEngine.Networking;

//this script does not communicate directly with other RaftCombat scripts
//this is possible since the damage is done by triggers of projectiles
[RequireComponent(typeof(RaftMovement))]
public class RaftCombat : RaftActionPerformer {

	private RaftMovement moveScript;
	private Vector3 lookDir{
		get{
			return moveScript.lookDir;
		}
	}

	[SerializeField]private GameObject projectilePrefab;
	[SerializeField]private int projectileDamage = 2;
	[SerializeField]private float shootSpeed = 3f;
	[SerializeField]private float projectileOffset = 0.6f;

	protected override void Start(){
		base.Start ();
		moveScript = GetComponent<RaftMovement> ();
	}

	public override void EvaluateInput (out bool succes) {
		if (Input.GetKeyUp (KeyCode.Space)) {
			succes = true;

			if (isServer)
				ServerShoot ();
			else
				CmdLetServerShoot ();
		} 
		else {
			succes = false;
		}
	}

	//check if we are being hit
	private void OnTriggerEnter2D(){
		
	}

	[Command]
	private void CmdLetServerShoot(){
		ServerShoot ();
	}

	private void ServerShoot(){
		Vector3 spawnPos = transform.position + lookDir * projectileOffset;
		GameObject projectile = GameObject.Instantiate (projectilePrefab, spawnPos, Quaternion.identity);

		Projectile projectileScript = projectile.GetComponent<Projectile> ();
		projectile.GetComponent<Rigidbody2D> ().velocity = lookDir * shootSpeed;
		projectileScript.myRaft = transform;
		projectileScript.damage = projectileDamage;

		NetworkServer.Spawn (projectile);
		projectileScript.onDestroy += ServerOnActionFinished;
	}
}
