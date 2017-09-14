using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable{
	void OnDeath();
	bool TakeDamage(IDamager damager);
}

public interface IDamager{
	///<returns><c>true</c>, if damage was fatal, <c>false</c> otherwise.</returns>
	int Damage{ 
		get; 
		set;
	}
}
