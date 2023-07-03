using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntRepositionState : GruntBaseState
{
    List<Node> validNodes = new List<Node>();
    Node nodeCheck;
    Node nodeToGoTo;
    bool reachedNode;
    public override void EnterState(GruntStateMachine grunt)
    {
        reachedNode = false;
        int random = Random.Range(0, grunt.totalNodes.Count);
        nodeToGoTo = grunt.totalNodes[random];
        grunt.agent.SetDestination(nodeToGoTo.transform.position);
    }

    public override void UpdateState(GruntStateMachine grunt)
    {
        reachedNode = nodeToGoTo.hasReached;
        if (reachedNode)
        {
            GetValidNodes(grunt);
            if(validNodes.Count > 0)
            {
                int random = Random.Range(0, validNodes.Count);
                nodeToGoTo = validNodes[random];
                grunt.agent.SetDestination(nodeToGoTo.transform.position);
            }
            reachedNode = false;
        }
    }

    public override void OnCollisionEnter(GruntStateMachine grunt, Collision collision)
    {

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

}
