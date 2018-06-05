using UnityEngine;
using UnityEngine.UI;

//used for either updating the password or the email
public class UpdateDataPanel : MonoBehaviour {

	private UserDataPanel userDataScript;//used for first validating the user
	private CanvasGroup panel;

	private const string USERPAGE_URL = "http://studenthome.hku.nl/~nathan.flier/RaftWars_DB/userData.php";
	private const string USERID_KEY = "userID";
	[SerializeField]private string update_Key = "newPassword";

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
		panel.DeActivate ();
		userDataScript.RestoreStandardView ();
	}

	public void ConfirmNewValue(){
		if (confirmNewValue != newValue) {
			userDataScript.ShowWarning("the input of these fields do not match");
		}
		else if (newValue == "") {
			userDataScript.ShowWarning("these fields should not be empty");
		}
		else {
			userDataScript.ShowSucces ("Updating...");
			int uID = GameManager.instance.userID;
			StartCoroutine(WebHandler.instance.ProcessWebRequest (USERPAGE_URL, userDataScript.ShowSucces, userDataScript.ShowWarning, 
				USERID_KEY, uID.ToString(), update_Key, newValue));

			if (update_Key == "newPassword") {
				GameManager.instance.UpdatePassword (newValue);
			}
		}
	}
}
