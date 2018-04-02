using UnityEngine;
using UnityEngine.UI;
using AI_UtilitySystem;

public class HealthBar : MonoBehaviour {

	private int maxHitPoints;
	private Image healthImg;

	private Transform cameraTransform;
	private Transform target;
	public bool isWorldRelative = true;
	[SerializeField]private float yOffset = 1f;

	private void Start(){
		healthImg = transform.GetChild (0).GetComponent<Image> ();
	}

	//initialize the healthbar behaviour:
	public void Init(CombatHandler _combatScript){
		transform.name = _combatScript.name + "_health";
		_combatScript.onHealthChanged += HealthChanged;
		target = _combatScript.transform;
		maxHitPoints = _combatScript.maxHitPoints;

		if (isWorldRelative) {
			cameraTransform = Camera.main.transform;
		}
	}

	//make sure the healthbar stays on top of the unit it belongs to:
	private void Update(){
		if (!isWorldRelative)
			return;
		
		Vector3 targetPos = target.position + Vector3.up * yOffset;
		transform.position = targetPos;

		Vector3 lookDir = transform.position - cameraTransform.position;
		lookDir.y = 0;
		transform.rotation = Quaternion.LookRotation (lookDir);
	}

	//make sure our health is properly filled:
	private void HealthChanged(int newHealth){ 
		if (newHealth == 0) {
			Destroy (gameObject);
			return;
		}

		float percentage = (float)newHealth / maxHitPoints;
		healthImg.fillAmount = percentage;
	}
}
