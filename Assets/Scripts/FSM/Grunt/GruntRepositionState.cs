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
        grunt.agent.SetDestination(nodeToGoTo.transform.position);
    }


    float reachNodeTimer = 0;
    float nodeOutOfLOSTimer = 0;

    float betweenShotsTimer = 0;
    public override void UpdateState(GruntStateMachine grunt)
    {
        reachedNode = nodeToGoTo.hasReached;
        if (reachedNode)
        {
            ReachedTimer(grunt);
        }
        if (!nodeToGoTo.isValid)
        {
            OutOfLOSTimer(grunt);
        }
        if (grunt.localInLOS && grunt.localInRange && grunt.ap.hasToken)
        {
            float randomShots;
            randomShots = Random.Range(grunt.shotsMin, grunt.shotsMax);
            for(int i = 0; i < randomShots; i++)
            {
                if (!grunt.localInLOS || !grunt.localInRange || !grunt.ap.hasToken) return;
                TimeInBetweenShotsTimer(grunt);
            }
        }
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
    void ClearTimers()
    {
        reachNodeTimer = 0;
        nodeOutOfLOSTimer = 0;
    }

}
