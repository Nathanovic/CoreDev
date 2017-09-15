using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IDamager {

	private Player player;

	public void Fire(){
		//deal Damage on hit to IDamageable
	}

	public DamageInfo damageInfo;

	int damage;
	public int Damage {
		get{ 
			return damageInfo.amount;
		}
		set{
			damage = value;	
		}
	}
}
