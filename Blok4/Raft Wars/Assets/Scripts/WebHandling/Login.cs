using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public delegate void JsonReply(string json);
public delegate void ErrorReply(string json);

//is at the login canvas object
//handles the login for the player using WebHandler
//gives all the data (if valid login) to the playerData script
public class Login : MonoBehaviour {

	[SerializeField]private int gameID = 1;
	private const string LOGIN_URL = "http://studenthome.hku.nl/~nathan.flier/RaftWars_DB/login.php";
	private const string USERID_KEY = "id";
	private const string USERNAME_KEY = "name";
	private const string SESSID_KEY = "PHPSESSID";

	[SerializeField]private UserStats userScript;

	[SerializeField]private InputField mailField; 
	[SerializeField]private InputField passwordField; 
	[SerializeField]private Text errorText; 
	private string filledMail = "";
	private string filledPassword = "";

	private CanvasGroup loginPanel; 
	[SerializeField]private NetworkHUD gameConnectScript;

	private void Start () {
		loginPanel = GetComponent<CanvasGroup> ();

		if (!GameManager.instance.loggedIn) {
			loginPanel.Activate ();
			filledMail = PlayerPrefs.GetString ("_mail");
			filledPassword = PlayerPrefs.GetString ("_password");
			mailField.text = filledMail;
			passwordField.text = filledPassword;
		}
		else {
			EnterConnectionHUD ();
		}
	}
	
	public void SetMail(string mail){
		filledMail = mail;
		errorText.enabled = false;
	}

	public void SetPassword(string password){
		filledPassword = password;
		errorText.enabled = false;
	}

	private void EnterConnectionHUD(){
		loginPanel.DeActivate ();
		gameConnectScript.EnableGameHUD (GameManager.instance.userName);		
	}

	//"mail@mail.com" "PassWooord"
	public void TryLogin(){
		PlayerPrefs.SetString ("_mail", filledMail);
		PlayerPrefs.SetString ("_password", filledPassword);
		StartCoroutine(WebHandler.instance.ProcessWebRequest (LOGIN_URL, HandleLogin, HandleLoginError, 
			"mail", filledMail, "password", filledPassword, "gameID", gameID.ToString()));
	}

	private void HandleLogin(string result){
		JObject jsonObject = JObject.Parse (result);

		int userID = jsonObject [USERID_KEY].ToObject<int> ();
		string userName = jsonObject [USERNAME_KEY].ToObject<string> ();
		string sessionID = jsonObject [SESSID_KEY].ToObject<string> ();

		GameManager.instance.PlayerLoggedIn (userName, userID, filledPassword);
		WebHandler.instance.SetUserSessionID (sessionID);

		EnterConnectionHUD ();
	}

	private void HandleLoginError(string error){
		errorText.enabled = true;
		errorText.text = error;
	}
}
