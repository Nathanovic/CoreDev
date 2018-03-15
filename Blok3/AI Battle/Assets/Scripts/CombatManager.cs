using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

	private static CombatManager _instance;
	public static CombatManager Instance{
		get{
			if (_instance == null) {
				GameObject go = new GameObject ("Combat Manager");
				_instance = go.AddComponent<CombatManager> ();
			}

			return _instance;
		}
	}

	public List<IAttackable> potentialTargets = new List<IAttackable>();

	public void RegisterPotentialTarget(IAttackable t){
		potentialTargets.Add (t);
	}

	public void PotentialTargetDied(IAttackable t){
		potentialTargets.Remove (t);
	}
}
