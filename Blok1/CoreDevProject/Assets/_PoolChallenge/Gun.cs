using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKU.KGDEV1.Start {
	public class Gun : MonoBehaviour {
		BulletPool bulletPool;
		const float REFIRE_TIME = .1f;

		float lastFireTime = 0;
		Transform mTransform;

		public int bulletCount = 10;
		public float bulletLifeTime = 1f;

		void Awake() {
			//don't call transform constantly (slow)
			//make sure to set mTransform during Awake (not Start, which may not happen for disabled objects)
			mTransform = transform;
		}

		void Start(){
			bulletPool = FindObjectOfType<BulletPool> ();
			bulletPool.InitGun (bulletCount);
		}

		// Update is called once per frame
		void LateUpdate() {
			//get button, fire
			if ( Input.GetMouseButton(0)) {
				Fire();
			}
		}

		void Fire() {
			if (Time.time - lastFireTime >= REFIRE_TIME) {
				//activate current bullet, point it in the right direction
				Bullet bullet = bulletPool.ActivateBullet(bulletLifeTime);
				if(bullet == null){
					Debug.LogWarning ("no bullet available...");
					return;
				}

				bullet.Fired(mTransform.position, mTransform.forward);

				//register that we fired
				lastFireTime = Time.time;
			}
		}
	}
}