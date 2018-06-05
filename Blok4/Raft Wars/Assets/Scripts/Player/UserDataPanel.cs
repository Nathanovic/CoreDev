using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UserDataPanel : MonoBehaviour {

	private CanvasGroup panel;
	[SerializeField]private CanvasGroup passwordCheckPanel;

	[SerializeField]private Text warningText;

	private string filledInCheckPassword;
	private UnityAction validPasswordCallback;

	private void Start () {
		panel = GetComponent<CanvasGroup> ();
		panel.DeActivate ();
		passwordCheckPanel.DeActivate ();
	}
	
	public void Activate(){
		panel.Activate ();
	}

	public void Deactivate(){
		panel.DeActivate ();
		warningText.enabled = false;
	}

	public void RequestChangeUserData(UnityAction validUserCallback){
		validPasswordCallback = validUserCallback;
		passwordCheckPanel.Activate ();
	}

	public void OnFilledPasswordEdited(string newFilledPassword){
		filledInCheckPassword = newFilledPassword;
		warningText.enabled = false;
	}

	public void EvaluatePassword(){
		if (filledInCheckPassword == GameManager.instance.password) {
			validPasswordCallback ();
		} 
		else {
			warningText.enabled = true;
			warningText.text = "invalid password";
		}
	}
}
