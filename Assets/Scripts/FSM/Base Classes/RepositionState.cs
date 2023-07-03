using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "FSM/Actions/Reposition")]
public class RepositionState : FSMAction
{
    public override void Execute(BaseStateMachine stateMachine)
    {
        var navMeshAgent = stateMachine.GetComponent<NavMeshAgent>();
        var grids = stateMachine.GetComponent<Node>();
        var nodes = stateMachine.GetComponent<Node>();

        //if (nodes)
    }
}
