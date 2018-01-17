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
	public virtual void OnEnter (AI target, GameObject otherObject){
		foreach (Condition condition in nextStateConditions) {
			condition.Activate (otherObject);
		}
	}
	public abstract void Run (AI target);
	public virtual void OnExit (AI target){
		foreach (Condition condition in nextStateConditions) {
			condition.Deactivate ();
		}
	}
}