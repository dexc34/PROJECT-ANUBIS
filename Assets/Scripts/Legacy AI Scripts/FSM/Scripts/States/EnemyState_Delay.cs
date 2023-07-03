using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to delay the time for enemy to go to next state
public class EnemyState_Delay : IState
{
    private float waitForSeconds;
    private float deadline;

    public EnemyState_Delay(float waitForSeconds)
    {
        this.waitForSeconds = waitForSeconds;
    }

    public void OnEnter()
    {
        deadline = Time.time + waitForSeconds;
    }

    public void OnExit()
    {
        Debug.Log("EnemyDelay onExit");
    }

    public void Tick()
    {

    }

    public Color GizmoColor()
    {
        return Color.white;
    }

    public bool IsDone()
    {
        return Time.time >= deadline; 
    }

}
