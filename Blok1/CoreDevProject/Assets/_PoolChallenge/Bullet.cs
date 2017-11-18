using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKU.KGDEV1.Start {
	public delegate void BulletDelegate(GameObject self);

	public class Bullet : MonoBehaviour {
		const float SPEED = 10;
		private float MAX_LIFETIME = 3;

		public Transform mTransform;
		float fireTime = 0;

		public event BulletDelegate onDisableSelf;
		private BulletPool poolScript;

		void Awake() {
			mTransform = transform;
		}

		void Start(){
			onDisableSelf += DisableSelf;
		}

		public void Initialize(float lifeTime){
			MAX_LIFETIME = lifeTime;
		}

		public void Fired( Vector3 position, Vector3 forward ) {
			//set our velocity, etc.
			mTransform.position = position;
			mTransform.forward = forward;
			fireTime = Time.time;
		}

		void Update() {
			mTransform.Translate(Vector3.forward * Time.deltaTime * SPEED, Space.Self);
			//optional: add some kind of timer to disable the bullet automatically (don't destroy it!)
			if ( Time.time - fireTime > MAX_LIFETIME ) {
				if (onDisableSelf != null) {
					onDisableSelf (gameObject);
				}
			}
		}

		void DisableSelf(GameObject self){
			self.SetActive(false);		
		}
	}
}
