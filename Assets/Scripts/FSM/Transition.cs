using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Transtion")]
public sealed class Transition : ScriptableObject
{
    public Decision Decision;
    public BaseState TrueState;
    public BaseState FalseState;

    public void Execute(BaseStateMachine stateMachine)
    {
        if (Decision.Decide(stateMachine) && !(TrueState is RemainInState))
            stateMachine.CurrentState = TrueState;
        else if (!(FalseState is RemainInState))
            stateMachine.CurrentState = FalseState;
    }
}
