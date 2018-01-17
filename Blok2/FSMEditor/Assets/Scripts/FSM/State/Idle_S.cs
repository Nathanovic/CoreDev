
[System.Serializable]
public class Idle_S : State{
	public override void Run (AI target){
		target.Idle ();
	}
}