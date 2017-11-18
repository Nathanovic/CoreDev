using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	public Transform target;
	public float rotation;

	Vector3 origin;
	Vector3 targetPos;
	Vector3 soughtPos;

	void Start(){
		origin = Vector3.zero;
		targetPos = target.position;
		rotation = CalculateAngle (transform.position, targetPos);
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.A)) {
			origin = Vector3.zero;
			targetPos = target.position;
			AngleCalculatorSetup ();
		}
		else if (Input.GetKeyUp (KeyCode.B)) {
			origin = target.position;
			targetPos = target.position + Vector3.right;
			AngleCalculatorSetup ();
		}
	}

	void AngleCalculatorSetup(){
		rotation = CalculateAngle (transform.position, targetPos);
		float magn = (targetPos - transform.position).magnitude;
		float newAngle = rotation + 40;
		soughtPos = target.InverseTransformDirection (Quaternion.AngleAxis (newAngle, transform.right) * target.forward * magn);
		Debug.Log ("new angle: " + CalculateAngle (transform.position, soughtPos + Vector3.right));  
	}

	float CalculateAngle(Vector3 myPos, Vector3 targetPos){
		return Vector3.Angle (myPos, targetPos);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (origin, transform.position);
		Gizmos.DrawLine (origin, targetPos);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere (origin, 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (targetPos, 0.1f);

		if (soughtPos != Vector3.zero) {
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere (soughtPos, 0.1f);
			Gizmos.DrawLine (origin, soughtPos);
		}
	}

	float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
	{
		Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
		float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
		return Vector2.Angle(vec1, vec2) * sign;
	}
}
