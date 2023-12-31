using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntRepositionState : GruntBaseState
{
    List<Node> validNodes = new List<Node>();
    Node nodeToGoTo;
    bool reachedNode;

    public override void EnterState(GruntStateMachine grunt)
    {
        reachedNode = true;
        int random = Random.Range(0, grunt.totalNodes.Count);
        nodeToGoTo = grunt.totalNodes[random];

        //grunt.animator.SetBool(grunt.isMovingHash, true);

        grunt.agent.SetDestination(nodeToGoTo.transform.position);
    }


    float reachNodeTimer = 0;
    float nodeOutOfLOSTimer = 0;

    float betweenShotsTimer = 0;
    public override void UpdateState(GruntStateMachine grunt)
    {
        //HANDLES REPOSITIONING
        reachedNode = nodeToGoTo.hasReached;
        if (reachedNode)
        {
            ReachedTimer(grunt);
        }
        if (!nodeToGoTo.isValid)
        {
            OutOfLOSTimer(grunt);
        }


        //HANDLES SHOOTING
        if (grunt.localInLOS && grunt.localInRange && grunt.attackCooldownOver && grunt.ap.hasToken)
        {
            ShootWhileMoving(grunt);
        }
        else if (grunt.localCloseRange && grunt.localInLOS && grunt.attackCooldownOver && grunt.ap.hasToken)
        {
            ShootWhileMoving(grunt);
        }

        if (!grunt.attackCooldownOver)
        {
            AttackCooldownTimer(grunt);
        }
    }

    void ShootWhileMoving(GruntStateMachine grunt)
    {
        grunt.TriggerShotsWhileMoving();
        //AttackCooldownTimer(grunt);
    }

    public override void OnCollisionEnter(GruntStateMachine grunt, Collision collision)
    {
        //do nothing 
    }

    void GetValidNodes(GruntStateMachine grunt)
    {
        validNodes.Clear();
        foreach(Node nodesInList in grunt.totalNodes)
        {
            nodesInList.CheckRange(grunt.perferedRangeMin, grunt.perferedRangeMax);
            if(nodesInList.isValid)
            {
                validNodes.Add(nodesInList);
            }
        }
    }

    void ReachedTimer(GruntStateMachine grunt)
    {
        //grunt.animator.SetBool(grunt.isMovingHash, false);

        reachNodeTimer += Time.deltaTime;
        if (reachNodeTimer > grunt.moveDelay)
        {
            FindNewNode(grunt);
            reachNodeTimer = 0;
        }
    }

    void OutOfLOSTimer(GruntStateMachine grunt)
    {
        nodeOutOfLOSTimer += Time.deltaTime;
        if (nodeOutOfLOSTimer > grunt.switchNodeDelay)
        {
            FindNewNode(grunt);
            nodeOutOfLOSTimer = 0;
        }
    }

    void FindNewNode(GruntStateMachine grunt)
    {
        GetValidNodes(grunt);
        if (validNodes.Count > 0)
        {
            int random = Random.Range(0, validNodes.Count);
            nodeToGoTo = validNodes[random];
            grunt.agent.SetDestination(nodeToGoTo.transform.position);

            //grunt.animator.SetBool(grunt.isMovingHash, true);
        }
        reachedNode = false;
    }

    void FireShotsWhileMoving(GruntStateMachine grunt)
    {
        grunt.ShootState.EnterState(grunt);
    }

    void TimeInBetweenShotsTimer(GruntStateMachine grunt)
    {
        betweenShotsTimer += Time.deltaTime;
        if (betweenShotsTimer > grunt.timeInBetweenShots)
        {
            FireShotsWhileMoving(grunt);
            betweenShotsTimer = 0;
        }
    }

    public float timeSinceLastAttack = 0;
    void AttackCooldownTimer(GruntStateMachine grunt)
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack > grunt.delayBetweenAttacks)
        {
            grunt.attackCooldownOver = true;
            timeSinceLastAttack = 0;
        }
    }
    void ClearTimers()
    {
        reachNodeTimer = 0;
        nodeOutOfLOSTimer = 0;
    }

}
