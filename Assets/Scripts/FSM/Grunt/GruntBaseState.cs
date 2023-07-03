using UnityEngine;

public abstract class GruntBaseState 
{
    public abstract void EnterState(GruntStateMachine grunt);

    public abstract void UpdateState(GruntStateMachine grunt);

    public abstract void OnCollisionEnter(GruntStateMachine grunt, Collision collision);
}
