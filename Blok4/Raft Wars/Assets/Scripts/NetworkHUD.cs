using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkHUD : MonoBehaviour {
	private NetworkManager manager;
	public string homeIP = "192.168.178.234";
	public string hkuIP = "xxxxxxx";
	public Text connText;

	void Start () {
		manager = GetComponent<NetworkManager> ();
	}

	public void StartHost(){
		manager.StartHost();
		connText.text = "Setting up host...";
	}

	public void StartClientHome(){
		manager.networkAddress = homeIP;
		StartClient ();
	}
	public void StartClientCustom(){
		StartClient ();
	}
	public void StartClientHKU(){
		manager.networkAddress = hkuIP;
		StartClient ();
	}

	private void StartClient(){
		manager.StartClient ();	
		connText.text = "Connecting to " + manager.networkAddress + "...";
	}

	public void SetIP(string newIP){
		manager.networkAddress = newIP;
	}
}