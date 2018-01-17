
[System.Serializable]
public class Patrol_S : State{

	public override void OnEnter(AI target){
		base.OnEnter (target);
		target.PreparePatrol ();
	}

	public override void Run (AI target){
		target.Patrol ();
	}
}