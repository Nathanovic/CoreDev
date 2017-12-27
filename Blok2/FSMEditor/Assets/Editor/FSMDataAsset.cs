using UnityEngine;
using UnityEditor;

public class FSMDataAsset : MonoBehaviour {
	[MenuItem("Assets/Create/FSMData")]
	public static void CreateAsset(){
		ScriptableObjectUtility.CreateAsset<FSMData> ();
	}
}
