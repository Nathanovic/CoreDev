
[System.Serializable]
public class Attack_S : State{

	IDamageable damageableTarget;

	public override void OnEnter (AI target, UnityEngine.GameObject otherObject) {
		base.OnEnter (target, otherObject);
		damageableTarget = otherObject.GetComponent<IDamageable> ();
		target.PrepareAttack (damageableTarget);
	}

	public override void Run (AI target){
		target.Attack (damageableTarget);
	}

	public override void OnExit (AI target) {
		base.OnExit (target);
		target.StopAttack (damageableTarget);
	}
}