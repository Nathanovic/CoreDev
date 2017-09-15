using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looper : MonoBehaviour {
	public int returnValue = 1;
	public int maxReturnValue = 200;
	void Start () {
		print(returnValue);
		returnValue++;
		if(returnValue <= maxReturnValue) 
			Start ();
	}
}
