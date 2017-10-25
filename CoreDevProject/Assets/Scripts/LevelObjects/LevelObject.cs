using UnityEngine;
using System.Collections;

public class LevelObject : MonoBehaviour {
	
	protected SpriteRenderer myRenderer;
	protected Transform loadbar;

	protected bool active;//active if someone is standing on me and I can do something with him

	protected float loadPercentage = 0;//range: 0 to 1
	private Vector3 loadbarScale;
	protected Vector3 LoadbarScale{
		get{
			Vector3 barBoundsSize = loadbarScale;
			barBoundsSize.x = loadPercentage;
			return barBoundsSize;
		}
	}

	protected virtual void Start () {
		myRenderer = GetComponent<SpriteRenderer> ();
		loadbar = transform.GetChild(0).transform;
		loadbarScale = loadbar.localScale;
		loadbar.localScale = LoadbarScale;		
	}
	
	protected IEnumerator LoadBar(float time){
		while (loadPercentage < 1f && active) {
			ModifyLoadbarWidth (Time.deltaTime / time);
			yield return null;
		}
	}

	protected void ModifyLoadbarWidth(float modifyValue){
		float newValue = loadPercentage + modifyValue;
		loadPercentage = Mathf.Clamp (newValue, 0f, 1f);
		loadbar.localScale = LoadbarScale;
	}

	protected void ResetLoadbarWidth(){
		loadPercentage = 0f;
		loadbar.localScale = LoadbarScale;
	}
}
