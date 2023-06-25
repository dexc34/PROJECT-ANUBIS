using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class EnemyReferences : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public EnemyShooter shooter;

    [Header("Stats")]
    //Delays PathUpdate to calculate every 0.2 seconds
    public float pathUpdateDelay = 0.2f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shooter = GetComponent<EnemyShooter>();
    }

  
}
