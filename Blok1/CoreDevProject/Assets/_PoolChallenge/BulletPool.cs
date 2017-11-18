using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HKU.KGDEV1.Start {
	[System.Serializable]
	public class BulletPool : MonoBehaviour {

		private int bulletCount;
		public GameObject bulletPrefab;
		public Stack<GameObject> bullets = new Stack<GameObject>();

		public void InitGun(int gunBulletCount){
			int prevBulletCount = bulletCount;
			bulletCount += gunBulletCount;

			for(int i = prevBulletCount; i < bulletCount; i ++){
				GameObject b = GameObject.Instantiate (bulletPrefab, transform);
				b.SetActive (false);
				bullets.Push (b);

				Bullet bullet = b.GetComponent<Bullet> ();
				bullet.onDisableSelf += RegisterBullet;
			}
		}

		public void RegisterBullet(GameObject b){
			bullets.Push (b);
		}

		public Bullet ActivateBullet(float lifeDuration){
			Bullet bullet = null;
			if (bullets.Count > 0) {
				GameObject bulletObj = bullets.Pop ();
				bulletObj.SetActive (true);

				bullet = bulletObj.GetComponent<Bullet> ();
				bullet.Initialize (lifeDuration);
			}

			return bullet;
		}
	}
}
