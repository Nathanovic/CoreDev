using System.Collections;
using UnityEngine;

public class CameraMouseLook : MonoBehaviour {

	private Vector2 mouseLook;
	private Vector2 smoothV;
	[SerializeField]private bool invertedX;
	[SerializeField]private float sensitivity = 5f;
	[SerializeField]private float smoothing = 2f;
	[SerializeField]private float upYMax = 90f;
	[SerializeField]private float downYMax = -90f;
	private Transform player;

	void Start () {
		player = transform.parent;
	}

	void Update () {
		Vector2 md = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
		md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
		md.x = invertedX ? (md.x * -1) : md.x;

		smoothV.x = Mathf.Lerp (smoothV.x, md.x, 1f / smoothing);
		smoothV.y = Mathf.Lerp (smoothV.y, md.y, 1f / smoothing);
		mouseLook += smoothV;
		mouseLook.y = Mathf.Clamp (mouseLook.y, downYMax, upYMax);

		transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);
		player.localRotation = Quaternion.AngleAxis (-mouseLook.x, Vector3.up);
	}
}
