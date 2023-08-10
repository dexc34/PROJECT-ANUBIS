using UnityEngine;
//abstract class for grunt actions to override
public class GruntSpawnState : GruntBaseState
{
    public bool isSpawning;
    public override void EnterState(GruntStateMachine grunt)
    {

    }

    public override void UpdateState(GruntStateMachine grunt)
    {
        if (isSpawning)
        {
            grunt.spawnParticle.Play();
        }
    }

    public override void OnCollisionEnter(GruntStateMachine grunt, Collision collision)
    {

    }
}
