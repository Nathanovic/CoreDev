
[System.Serializable]
public class Patrol_S : State{
	public override void Run (AI target){
		target.Patrol ();
	}
}