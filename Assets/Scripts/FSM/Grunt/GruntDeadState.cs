using UnityEngine;
//abstract class for grunt actions to override
public class GruntDeadState : GruntBaseState
{
    public override void EnterState(GruntStateMachine grunt)
    {
        grunt.StopAllCoroutines();
        grunt.ap.attackPriority = 50000000000000;
        grunt.ragdoll.EnableRagdoll();

        grunt.health.EnemyDie();
    }

    public override void UpdateState(GruntStateMachine grunt)
    {

    }

    public override void OnCollisionEnter(GruntStateMachine grunt, Collision collision)
    {

    }
}
