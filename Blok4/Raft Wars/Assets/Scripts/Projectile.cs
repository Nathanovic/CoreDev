using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour {

	public int damage;
	public Vector3 movement;
	[SerializeField]private float moveSpeed;
	public UnityAction reachedTargetCallback;

	private void Update(){ 
		
	}
}
