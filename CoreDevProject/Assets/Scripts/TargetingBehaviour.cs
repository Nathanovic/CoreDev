using UnityEngine;
using System.Collections;

//deze class houdt bij of de AI goed is of slecht en bepaald op basis hiervan wat 
//er gebeurd on collision, of de AI een target moet zoeken, wat de target van deze AI kan worden, 
//en vind op basis van de target het path(= typeof(Node[])) naar de target
public class TargetingBehaviour{

	private TargetStrategy currentBehaviour;

	protected Pathfinding pathfinder;
	protected Transform target;

	public TargetingBehaviour(TargetStrategy myStrategy){
		currentBehaviour = myStrategy;
	}

	public void UpdateTargetingBehaviour(TargetStrategy newStrategy){
		currentBehaviour = newStrategy;
	}

	public Node[] GetPathToTarget(){
		return new Node[2];
	}

	public void Collide (Collider other){
		currentBehaviour.Collide (other);
	}

}

public abstract class TargetStrategy{

	public abstract void SetTarget (Transform _target);
	public abstract void MoveToTarget ();
	public abstract void Collide (Collider other);
}

//probeer speler(of goede AI's) op te eten
public class EvilTargeting : TargetStrategy{

	public override void SetTarget (Transform _target){

	}

	public override void MoveToTarget(){

	}

	public override void Collide(Collider other){//eet speler op

	}
}

//probeer cookies op te eten
public class FriendlyTargeting : TargetStrategy{

	public override void SetTarget (Transform _target){

	}

	public override void MoveToTarget(){

	}

	public override void Collide(Collider other){//eet cookie op

	}
}