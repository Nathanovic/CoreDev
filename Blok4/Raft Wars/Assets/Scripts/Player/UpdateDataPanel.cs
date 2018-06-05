using UnityEngine;
using UnityEngine.UI;

//used for either updating the password or the email
public class UpdateDataPanel : MonoBehaviour {

	private UserDataPanel userDataScript;//used for first validating the user
	private CanvasGroup panel;
	[SerializeField]private Text warningText;

	private string newValue;
	private string confirmNewValue;

	private void Start(){
		panel = GetComponent<CanvasGroup> ();
		userDataScript = transform.parent.GetComponent<UserDataPanel> ();
		panel.DeActivate ();
	}

	public void UpdateData(){
		userDataScript.RequestChangeUserData (OnValidUserAttempt);
	}

	private void OnValidUserAttempt(){
		panel.Activate ();
	}

	public void UpdateNewValue(string v){
		newValue = v;
		warningText.enabled = false;
	}

	public void UpdateConfirmedValue(string v){
		confirmNewValue = v;
		warningText.enabled = false;
	}

	public void Cancel(){
		warningText.enabled = false;
	}

	public void ConfirmNewValue(){
		if (confirmNewValue != newValue) {
			warningText.enabled = true;
			warningText.text = "the input of these fields do not match";
		} 
		else {
			Debug.Log ("confirm: " + newValue);
		}
	}
}
