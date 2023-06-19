using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain_Smart : MonoBehaviour
{

    private EnemyReferences enemyReferences;
    private StateMachine stateMachine;

    void Start()
    {
        enemyReferences = GetComponent<EnemyReferences>();

        stateMachine = new StateMachine();

        //in this demo, we have only one area
        CoverArea coverArea = FindObjectOfType<CoverArea>();

        //STATES
        var runToCover = new EnemyState_RunToCover(enemyReferences, coverArea);
        //TRANSITIONS

        //START STATE
        stateMachine.SetState(runToCover);
        //FUNCTIONS & CONDITIONS

        //these are helper functions that make adding transitions shorter and more readable
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
        
    }

    void Update()
    {
        stateMachine.Tick();
    }

    private void OnDrawGizmos()
    {
        //since each state implements "IState" we can get current gizmo color for that state (good for debugging)
        if(stateMachine != null)
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }
    }
}
