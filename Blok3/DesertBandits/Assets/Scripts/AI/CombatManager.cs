using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

	private static CombatManager _instance;
	public static CombatManager Instance{
		get{
			return _instance;
		}
	}

	public List<CombatHandler> potentialTargets = new List<CombatHandler>();

	[SerializeField]private Transform screenCanvas;
	[SerializeField]private Transform worldCanvas;
	[SerializeField]private HealthBar healthBar;

	//this is called first due to the script execution order
	private void Awake(){
		_instance = this;
	}

	public void InitializeFighter(CombatHandler fighter, HealthBar customHealthBar = null){
		HealthBar healthBarPrefab = (customHealthBar != null) ? customHealthBar : healthBar;
		Transform healthBarParent = healthBarPrefab.isWorldRelative ? worldCanvas : screenCanvas;

		fighter.initialized = true;
		potentialTargets.Add (fighter);
		HealthBar newHealthBar = GameObject.Instantiate(healthBarPrefab, healthBarParent) as HealthBar;
		newHealthBar.Init (fighter);
	}

	public void FighterDied(CombatHandler fighter){
		potentialTargets.Remove (fighter);
	}
}
