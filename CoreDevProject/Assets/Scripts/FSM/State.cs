using UnityEngine;

public delegate void StateEvent(State newState);

public abstract class State {
	public StateEvent onState;

	public abstract void Start (Character target);
	public virtual void Run (Character target){}
	public virtual void Complete (Character target){}
}

public class InControl : State {
	public override void Start (Character target){
		//ai.Stop();
	}
	public override void Run(Character target){
		target.ControlMe ();
		bool foundTarget = false;//target.LookForPlayer();
		if (foundTarget) {
			onState (new Eating());
		}
	}
	public override void Complete(Character target){
	}
}

public class Eating : State {
	public override void Start (Character target){
		//ai.MovementSetup();
	}
	public override void Run(Character target){
		target.ControlMe ();
	}
	public override void Complete(Character target){
	}
}

public class Dying : State {
	public override void Start(Character target){
		target.EndMe ();
	}
}