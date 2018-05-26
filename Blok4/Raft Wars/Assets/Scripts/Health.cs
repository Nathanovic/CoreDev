using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

	[SerializeField]private int maxHealth = 3;
	[SyncVar(hook = "OnChangeHealth")]
	private int currentHealth;

	[SerializeField]private RectTransform healthBarFill;
	private Vector2 healthBarMaxFill;

	private void Start(){
		currentHealth = maxHealth;
		healthBarMaxFill = healthBarFill.sizeDelta;
	}

	public void TakeDamage(int amount){
		if (!isServer)
			return;

		currentHealth -= amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
		}

		Vector2 healthBarSize = healthBarMaxFill;
		healthBarSize.x *= (currentHealth / maxHealth);
		healthBarFill.sizeDelta = healthBarSize;
	}

	private void OnChangeHealth(int currentHealth){
		Vector2 healthBarSize = healthBarMaxFill;
		healthBarSize.x *= (currentHealth / maxHealth);
		healthBarFill.sizeDelta = healthBarSize;
	}
}
