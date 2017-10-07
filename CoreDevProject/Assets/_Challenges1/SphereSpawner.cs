using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour {
	public GameObject spherePrefab;
	public int sphereAmount = 10;
	public float sphereDistance = 1.5f;

	void Start () {
		float sphereAngleStep = 360f / sphereAmount * Mathf.Deg2Rad;
		float schuineZijde = 1f;

		for (int i = 0; i < sphereAmount; i++) {
			float sphereAngle = sphereAngleStep * i;

			float cosAngle = Mathf.Cos (sphereAngle);
			float aanliggendeZijde = cosAngle * schuineZijde;

			float sinAngle = Mathf.Sin (sphereAngle);
			float overstaandeZijde = sinAngle * schuineZijde;
			
			Vector3 angleDirection = new Vector3 (aanliggendeZijde, overstaandeZijde, 0);
			SpawnSphere (angleDirection);
		}
	}

	void SpawnSphere(Vector3 spawnDir){
		GameObject.Instantiate (spherePrefab, spawnDir * sphereDistance, Quaternion.identity);
	}
}
