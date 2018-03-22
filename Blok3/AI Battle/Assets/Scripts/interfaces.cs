using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public interface IAttackable{
	void ApplyDamage (int dmg);
	bool ValidTarget ();//can be set to false if health equals zero
	Vector2 Position ();
	Vector2 PrevPosition();//used to calculate speed towards another agent
}