using UnityEngine;

public delegate void StateEvent(StateName stateKey);

//states must be re-usable!
public abstract class State {
	protected AI baseAI;
	public StateEvent onState;

	public virtual void Init (AI _target){
		baseAI = _target;
	}

	public virtual void Start (){}
	public virtual void Run (){}
	public virtual void Complete (){}

	public virtual void Interrupt(){}
}

public enum StateName{
	patrolling,
	chasing,
	capturing
}