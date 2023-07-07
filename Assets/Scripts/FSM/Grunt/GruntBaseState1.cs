using UnityEngine;
//abstract class for grunt actions to override
public abstract class GruntBaseState 
{
    public abstract void EnterState(GruntStateMachine grunt);

    public abstract void UpdateState(GruntStateMachine grunt);

    public abstract void OnCollisionEnter(GruntStateMachine grunt, Collision collision);
}
