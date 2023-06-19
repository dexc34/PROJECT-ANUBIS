using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Cover : IState
{
    private EnemyReferences enemyReferences;
    private CoverArea coverArea;

    public EnemyState_Cover(EnemyReferences enemyReferences)
    {
        this.enemyReferences = enemyReferences;
    }

    //Must implement or define all methods in IState
    public void OnEnter()
    {
        enemyReferences.animator.SetBool("combat", true);

    }

    public void OnExit()
    {
        enemyReferences.animator.SetBool("combat", false);
    }

    public void Tick()
    {
        
    }

    public Color GizmoColor()
    {
        return Color.grey;
    }
}
