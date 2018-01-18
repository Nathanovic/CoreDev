using UnityEngine;

[System.Serializable]
public abstract class State : ScriptableObject {

	[SerializeField]private Condition[] nextStateConditions;

	public void InitConditions(Condition[] conditions){//called by StateNode
		nextStateConditions = conditions;
	}

	public virtual void OnEnter (AI target){
		foreach (Condition condition in nextStateConditions) {
			condition.Activate ();
		}
	}
	public virtual void OnEnter (AI target, GameObject otherObject){}
	public abstract void Run (AI target);
	public virtual void OnExit (AI target){
		foreach (Condition condition in nextStateConditions) {
			condition.Deactivate ();
		}
	}
}