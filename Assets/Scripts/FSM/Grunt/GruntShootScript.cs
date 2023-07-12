using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntShootScript : GruntBaseState
{
    public override void EnterState(GruntStateMachine grunt)
    {
        grunt.timeSinceLastAttack = 0;
        grunt.gun.EnemyShoot();
    }

    public override void UpdateState(GruntStateMachine grunt)
    {

    }

    public override void OnCollisionEnter(GruntStateMachine grunt, Collision collision)
    {

    }
}
