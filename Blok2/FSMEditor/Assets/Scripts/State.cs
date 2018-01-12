using UnityEngine;

[System.Serializable]
public abstract class State : ScriptableObject {

	private Condition[] nextStateConditions;

	public void InitConditions(params Condition[] conditions){
		nextStateConditions = conditions;
	}

	public virtual void OnEnter (AI target){
		foreach (Condition condition in nextStateConditions) {
			condition.Activate ();
		}
	}
	public abstract void Run (AI target);
	public virtual void OnExit (AI target){
		foreach (Condition condition in nextStateConditions) {
			condition.Deactivate ();
		}
	}
}

[System.Serializable]
public class Idle : State{
	public override void Run (AI target){
		target.Idle ();
	}
}
[System.Serializable]
public class Patrol : State{
	public override void Run (AI target){
		target.Patrol ();
	}
}
[System.Serializable]
public class Attack : State{
	public override void Run (AI target){
		target.Attack ();
	}
}
