using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public delegate void DamageDelegate(int dmg);

public class Health : NetworkBehaviour {

	[SerializeField]private int maxHealth = 3;
	[SyncVar(hook = "OnChangeHealth")]
	private int currentHealth;

	[SerializeField]private RectTransform healthBarFill;
	private Vector2 healthBarMaxFill;
	private bool healthInitialized;

	public event DamageDelegate onServerHealthChanged;
	public event SimpleDelegate onDie;

	private void Start(){
		healthBarMaxFill = healthBarFill.sizeDelta;		
		currentHealth = maxHealth;
		healthInitialized = true;
	}

	public void TakeDamage(int amount){
		if (!isServer)
			return;

		if (onServerHealthChanged != null) {
			onServerHealthChanged (amount);
		}

		currentHealth -= amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
			onDie ();
		}
	}

	private void OnChangeHealth(int currentHealth){
		if (!healthInitialized)
			return;

		float healthBarX = healthBarMaxFill.x * (float)currentHealth / maxHealth;
		healthBarFill.sizeDelta = new Vector2 (healthBarX, healthBarMaxFill.y);

		if (currentHealth == 0) {
			GetComponent<Animator> ().SetBool ("drowned", true);
		}
	}
}
