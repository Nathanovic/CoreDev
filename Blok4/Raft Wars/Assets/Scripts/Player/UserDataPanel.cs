using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UserDataPanel : MonoBehaviour {

	private CanvasGroup panel;
	[SerializeField]private CanvasGroup passwordCheckPanel;

	[SerializeField]private Button updatePsswrdButton;
	[SerializeField]private Button updateMailButton;
	[SerializeField]private Text warningText;
	[SerializeField]private Text succesText;

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
			updatePsswrdButton.interactable = false;
			updateMailButton.interactable = false;
			passwordCheckPanel.DeActivate ();
			validPasswordCallback ();
		} 
		else {
			ShowWarning("invalid password");
		}
	}

	public void RestoreStandardView(){
		updatePsswrdButton.interactable = true;
		updateMailButton.interactable = true;
		warningText.enabled = false;
		succesText.enabled = false;
	}

	public void ShowSucces(string result){
		succesText.text = result;
		succesText.enabled = true;
	}

	public void ShowWarning(string result){
		warningText.text = result;
		warningText.enabled = true;
	}
}
