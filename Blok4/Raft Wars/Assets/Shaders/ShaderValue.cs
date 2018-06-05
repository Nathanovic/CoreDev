using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderValue : MonoBehaviour {

	private Material m;
	public string name = "_Alpha";
	public string posName = "_ClipPosition";

	void Start () {
		m = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {
		m.SetFloat (name, Mathf.PingPong (Time.time, 1f));
		Shader.SetGlobalVector (posName, transform.position);
	}
}
