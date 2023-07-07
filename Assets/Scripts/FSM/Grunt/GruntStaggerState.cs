using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntStaggerState : GruntBaseState
{
    public override void EnterState(GruntStateMachine grunt)
    {
        grunt.animator.SetTrigger("isStaggered");
        grunt.animator.applyRootMotion = true;
        grunt.agent.isStopped = true;
    }

    public override void UpdateState(GruntStateMachine grunt)
    {

    }



    public override void OnCollisionEnter(GruntStateMachine grunt, Collision collision)
    {

    }
}
