using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public delegate void JsonReply(string json);

public class WebHandler : MonoBehaviour {

	private const string LOGIN_URL = "http://studenthome.hku.nl/~nathan.flier/loginPHP.php";
	private const string USERINFO_URL = "http://studenthome.hku.nl/~nathan.flier/loginPHP.php";
	private const string SESSION_KEY = "userID";
	private string sessionID = "userID";
	private string userSessionValue = "X";

	private const string USERID_KEY = "id";
	private const string USERNAME_KEY = "name";

	public UserData userData;

	private void Start(){
		StartCoroutine (ProcessWebRequest (LOGIN_URL, HandleLogin, "mail", "mail@mail.com", "password", "PassWooord"));
	}

	private IEnumerator ProcessWebRequest(string url, JsonReply callback, params string[] postArgs){
		List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
		formData.Add(new MultipartFormDataSection(SESSION_KEY + "=" + userSessionValue));

		WWWForm form = new WWWForm();
		form.AddField (SESSION_KEY, sessionID);

		for (int i = 0; i < postArgs.Length; i+= 2) {
			string postSection = "&" + postArgs [i] + "=" + postArgs [i + 1];
			formData.Add(new MultipartFormDataSection(postSection));

			form.AddField(postArgs[i], postArgs[i + 1]);
		}

		UnityWebRequest www = UnityWebRequest.Post(url, form);
		www.chunkedTransfer = false;
		yield return www.SendWebRequest ();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log (www.error);
		}  
		else {
			if (callback != null) {
				string jsonText = "[" + www.downloadHandler.text + "]";
				callback (jsonText);
			}
		}
	}

	private void HandleLogin(string result){
		JArray jsonResults = JArray.Parse (result);
		var jsonUserData = jsonResults [0];
		int userID = jsonUserData [USERID_KEY].ToObject<int> ();
		string userName = jsonUserData [USERNAME_KEY].ToObject<string> ();
		userSessionValue = userID.ToString();
		userData = new UserData (userID, userName);
	}
}

[System.Serializable]
public class UserData{
	public int id;
	public string name;

	public UserData(int _id, string _name){
		id = _id;
		name = _name;
	}
}
