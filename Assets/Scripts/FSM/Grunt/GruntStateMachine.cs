using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GruntStateMachine : MonoBehaviour
{
    //STATES//
    [HideInInspector] public GruntBaseState currentState;
    [HideInInspector] public GruntRepositionState RepositionState = new GruntRepositionState();
    [HideInInspector] public GruntShootScript ShootState = new GruntShootScript();

    //REFERENCES//
    [HideInInspector]public GameObject player;
    [HideInInspector] public Gun gun;
    [HideInInspector] public Health health;

    [HideInInspector] public AttackCoordinator ac;
    [HideInInspector] public AttackPriority ap;

    [SerializeField] NodeGroup group;
    [HideInInspector] public List<Node> totalNodes;

    [HideInInspector] public NavMeshAgent agent;

    //RANGE//
    [Header("Range")]
    [Tooltip("The minimum range the enemy wants to be in")]
    public float perferedRangeMin;
    [Tooltip("The maximum range the enemy wants to be in")]
    public float perferedRangeMax;

    //TIMER//
    [Header("Timers")]
    [Tooltip("How long an enemy stays on a node")]
    public float moveDelay = 2f;
    [Tooltip("How long a node has to be out of LOS before repositioning")]
    public float switchNodeDelay = 1f;

    //ENEMY BEHAVIOR// 
    [Header("Enemy Behaviour")]
    [SerializeField] LayerMask whatLayersBlock;
    [Tooltip("How many times the enemy should shoot in a single attack (minimum value)?")]
    [SerializeField] public float shotsMin = 1;
    [Tooltip("How many times the enemy should shoot in a single attack (maximum value)?")]
    [SerializeField] public float shotsMax = 1;
    [Tooltip("How long should the enemy wait before shooting again in a single attack?")]
    [SerializeField] public float timeInBetweenShots = 1;
    [Tooltip("How long should the enemy wait before initiating another attack?")]
    [SerializeField] public float delayBetweenAttacks = 2;
    [Tooltip("How tall is the enemy")]
    [SerializeField] float heightLOS = 1.75f;
    [Tooltip("How fast the enemy looks at the player")]
    [SerializeField] int lookSpeed = 5;
    [HideInInspector] public bool attackCooldownOver = true;

    //LOCAL POSITIONS AND ROTATIONS// 
    Vector3 localTargetDirection;
    Vector3 localLOSPosition;

    //ATTACK RELATED VARS//
    [Header("Attack Settings")]
    //counts how long its been since last attack
    [HideInInspector] public float timeSinceLastAttack = 0;
    [Tooltip("How much should time since last shot should effect how often this enemy attacks (multiplied)")]
    public float lastAttackMultiplier = 1;

    [Tooltip("How long should this enemy wait before trying to attack again")]
    public float attackCooldownTimer;

    //has this enemy requested?
    [HideInInspector] public bool hasRequested;
    //is this enemy allowed to attack?
    [HideInInspector] public bool canAttack;

    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        gun = GetComponent<Gun>();
        health = GetComponent<Health>();
        ac = GameObject.Find("Attack Coordinator").GetComponent<AttackCoordinator>();
        ap = GetComponent<AttackPriority>();

        totalNodes = group.nodesInGroup;
        currentState = RepositionState;

        //TEMP UNTIL SPAWN STATE IS ADDED
        //ap.AddThisToBackLog();

        currentState.EnterState(this);
    }

    private void Update()
    {
        LookAtPlayer();

        CalculateLocalLOS();

        CalculateLocalRange();
        CheckLocalRange();

        CalculateAttackPriority();

        currentState.UpdateState(this);

        if (health.currentHealth <= 0)
        {
            //a ludicrously high number so it doesnt pick this enemy to shoot ever again (TODO: bit of a bandaid fix, make a better solution later (maybe)
            ap.attackPriority = 50000000000000;
            health.EnemyDie();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }

    void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 relativePos = player.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = transform.localRotation;

            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime
                * lookSpeed);
        }
    }

    RaycastHit hit;
    [HideInInspector] public bool localInLOS;
    void CalculateLocalLOS()
    {
        localLOSPosition = transform.position + new Vector3(0, heightLOS, 0);
        localTargetDirection = (player.transform.position - localLOSPosition).normalized;
        if (Physics.Raycast(localLOSPosition, localTargetDirection, out hit, Mathf.Infinity, whatLayersBlock))
        {
            if (hit.collider.gameObject != player)
            {
                localInLOS = false;
            }
            else if (hit.collider.gameObject == player)
            {
                localInLOS = true;
            }
            else
            {
                localInLOS = false;
            }
        }
    }

    void CalculateLocalRange()
    {
        localRangeToPlayer = hit.distance;
    }

    [HideInInspector] public float localRangeToPlayer;
    [HideInInspector] public bool localInRange;
    public void CheckLocalRange()
    {
        if (localRangeToPlayer > perferedRangeMin && localRangeToPlayer < perferedRangeMax)
        {
            localInRange = true;
        }
        else
        {
            localInRange = false;
        }
    }

    void CalculateAttackPriority()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (localInLOS && localInRange)
        {
            ap.attackPriority = hit.distance - (timeSinceLastAttack * lastAttackMultiplier);
        }
        else
        {
            ap.attackPriority = 10000;
        }
    }

}
