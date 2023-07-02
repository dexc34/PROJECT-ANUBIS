using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntStateMachine : MonoBehaviour
{
    GruntBaseState currentState;
    GruntRepositionState RepositionState = new GruntRepositionState();
    GruntShootScript ShootState = new GruntShootScript();

    private void Start()
    {
        currentState = RepositionState;

        currentState.EnterState(this);
    }
}
