using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//is active for as long as the game is active
public class WebHandler : MonoBehaviour {

	private const string SESSION_KEY = "userID";
	private string sessionID = "_";

	public IEnumerator ProcessWebRequest(string url, JsonReply succesCallback, ErrorReply failCallback, params string[] postArgs){
		WWWForm form = new WWWForm();
		form.AddField (SESSION_KEY, sessionID);//session can be used to check if the user is logged in

		for (int i = 0; i < postArgs.Length; i+= 2) {
			form.AddField(postArgs[i], postArgs[i + 1]);
		}

		UnityWebRequest www = UnityWebRequest.Post(url, form);
		www.chunkedTransfer = false;
		yield return www.SendWebRequest ();

		if (www.isNetworkError || www.isHttpError) {
			Debug.Log (www.error);
		}  
		else {
			string webText = www.downloadHandler.text;
			if (webText.StartsWith ("[") || webText.StartsWith ("{")) {
				succesCallback (webText);
			}
			else {
				failCallback (webText);
			}
		}
	}

	public void SetUserSessionID(string id){
		sessionID = id;
	}
}
