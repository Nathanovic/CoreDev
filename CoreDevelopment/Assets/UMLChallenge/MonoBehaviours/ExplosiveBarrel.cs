using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : Entity, IDamager {

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
