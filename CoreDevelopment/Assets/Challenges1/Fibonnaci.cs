using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fibonnaci : MonoBehaviour {
	[Header("Press 'S' to solve")]
	public int fibonnaciN = 2;
	public int currentIndex;

	void Update () {
		if (Input.GetKeyUp (KeyCode.S)){
			currentIndex = 2;
			if (fibonnaciN <= 0)
				print ("Het is onmogelijk om het " + fibonnaciN + "de getal vinden!");
			else if (fibonnaciN < 3) 
				PrintResult(fibonnaciN - 1);
			else
				SolveFibonnaci ();

			print ("alternative recursive solution: " + fin(fibonnaciN - 1).ToString());
		}
	}
	
	void SolveFibonnaci(int startValue = 0, int previousValue = 1){
		currentIndex ++;
		int newValue = startValue + previousValue;
		if (currentIndex == fibonnaciN)
			PrintResult (newValue);
		else {
			SolveFibonnaci (previousValue, newValue);
		}
	}

	int fin(int n){
		if (n == 0)
			return 0;
		if (n == 1)
			return 1;
		return fin (n - 2) + fin (n - 1);
	}

	void PrintResult(int result){
		print ("Resultaat: " + result);
	}
}
