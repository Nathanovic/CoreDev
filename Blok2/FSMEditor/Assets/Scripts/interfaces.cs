using UnityEngine;

public interface IDamageable{
	void ApplyDamage(int dmg);
	event DefaultEvent onDie;
}