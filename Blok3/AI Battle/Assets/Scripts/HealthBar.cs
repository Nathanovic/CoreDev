using UnityEngine;
using UnityEngine.UI;
using AI_UtilitySystem;

public class HealthBar : MonoBehaviour {

	private AIStats statsModel;
	private RectTransform healthFill;

	private Transform target;
	public float yOffset = 1f;

	private void Start(){
		healthFill = transform.GetChild (0).GetComponent<RectTransform> ();
	}

	//initialize the healthbar behaviour:
	public void Init(AIBase baseScript){
		transform.name = baseScript.name + "_health";
		baseScript.onHealthChanged += HealthChanged;
		statsModel = baseScript.GetComponent<AIStats> ();
		target = baseScript.transform;
	}

	//make sure the healthbar stays on top of the unit it belongs to:
	private void Update(){
		Vector3 targetPos = target.position + Vector3.up * yOffset;
		transform.position = targetPos;
	}

	//make sure our health is properly filled:
	private void HealthChanged(int newHealth){ 
		if (newHealth == 0) {
			Destroy (gameObject);
			return;
		}

		float percentage = (float)newHealth / statsModel.maxHealth;
		Vector2 healthBarSize = healthFill.sizeDelta;
		healthBarSize.x = 100f * percentage;
		healthFill.sizeDelta = healthBarSize;
	}
}
