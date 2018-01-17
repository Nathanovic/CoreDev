using UnityEngine;

[System.Serializable]
public class Collide_C : Condition{

	public Collide_C(){
		requireTags = true;
		type = ConditionType.passGameObject;
	}

	public override void Activate(){
		eventScript.onCollisionEnter += EvaluateCollision;
	}
	public override void Deactivate (){
		eventScript.onCollisionEnter -= EvaluateCollision;
	}

	void EvaluateCollision(Collider2D other){
		foreach (string validTag in validTags) {
			if (other.tag == validTag) {
				TriggerCondition (other.gameObject);
			}
		}
	}
}