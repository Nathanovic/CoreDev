using UnityEngine;
using UnityEngine.Events;

public delegate void SimpleDelegate();

public class Projectile : MonoBehaviour {

	public int damage;
	public event SimpleDelegate onDestroy;

	private void Start(){
		Invoke ("DestroySelf", 2f);
	}

	private void DestroySelf(){
		if (onDestroy != null) {
			onDestroy ();
		}

		GameObject.Destroy (gameObject);
	}
}
