using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public interface IAttackable{
	void ApplyDamage (int dmg);
	bool ValidTarget ();//can be set to false if health equals zero
	Vector3 Position ();
}