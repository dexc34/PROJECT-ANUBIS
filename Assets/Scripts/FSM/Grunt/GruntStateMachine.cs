using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GruntStateMachine : MonoBehaviour
{
    GruntBaseState currentState;
    GruntRepositionState RepositionState = new GruntRepositionState();
    GruntShootScript ShootState = new GruntShootScript();

    [HideInInspector]public GameObject player;
    [SerializeField] NodeGroup group;
    [HideInInspector] public List<Node> totalNodes;
    [SerializeField] public NavMeshAgent agent;

    public float perferedRangeMin;
    public float perferedRangeMax;

    private void Start()
    {
        player = GameObject.Find("Player");
        totalNodes = group.nodesInGroup;
        currentState = RepositionState;

        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }
}
