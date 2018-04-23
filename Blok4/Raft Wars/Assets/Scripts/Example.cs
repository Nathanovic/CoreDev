//Attach this script to a GameObject
//Create a Text GameObject(Create>UI>Text) and attach it to the Text field in the Inspector window
//This script changes the Text depending on if a client connects or disconnects to the server

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Example : NetworkManager
{
	//Assign a Text component in the GameObject's Inspector
	public Text m_ClientText;

	//Detect when a client connects to the Server
	public override void OnClientConnect(NetworkConnection connection)
	{
		//Change the text to show the connection on the client side
		m_ClientText.text =  " " + connection.connectionId + " Connected!";
	}

	//Detect when a client connects to the Server
	public override void OnClientDisconnect(NetworkConnection connection)
	{
		//Change the text to show the connection loss on the client side
		m_ClientText.text = "Connection" + connection.connectionId + " Lost!";
	}
}