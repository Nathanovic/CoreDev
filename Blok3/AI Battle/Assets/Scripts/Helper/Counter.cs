using UnityEngine;

public delegate void SimpleDelegate();

public class Counter {

	public event SimpleDelegate onCount;
	private bool stopOnCount;
	private bool isCounting;
	private float startCountTime;
	private float countTime;

	public Counter(float _countTime = 1f, bool _stopOnCount = true){
		countTime = _countTime;
		stopOnCount = _stopOnCount;

		CountManager.Instance.InitCounter (this);
	}

	public void StartCounting(float _countTime){
		isCounting = true;
		countTime = _countTime;
		startCountTime = Time.time;
	}

	public void StartCounting(){
		isCounting = true;
		startCountTime = Time.time;
	}

	public void Count(){
		if ((Time.time - startCountTime) > countTime && isCounting) {
			ReachedCount ();
		}
	}

	void ReachedCount(){
		startCountTime = Time.deltaTime;
		onCount ();

		isCounting = !stopOnCount;
	}

	public void StopCounting(){
		isCounting = false;
	}
}