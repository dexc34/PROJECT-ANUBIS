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
    [HideInInspector] public GruntDeadState DeadState = new GruntDeadState();
    [HideInInspector] public GruntBaseState StaggerState = new GruntStaggerState();

    //REFERENCES//
    [Header("Refernces")]
    [HideInInspector] public GameObject player;
    [HideInInspector] public Gun gun;
    [HideInInspector] public Health health;

    [HideInInspector] public AttackCoordinator ac;
    [HideInInspector] public AttackPriority ap;

    [SerializeField] NodeGroup group;
    [HideInInspector] public List<Node> totalNodes;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public RagdollToggle ragdoll;
    [HideInInspector] LocomotionSimpleAgent mover;
    [HideInInspector] AgentLinkMover connector;
    [HideInInspector] CharacterController controller;
    [HideInInspector] Collider localCollider;
    [SerializeField] Collider hurtbox;


    //ANIMATION RELATED VARS
    [HideInInspector] public Animator animator;
    [HideInInspector] public int isMovingHash;

    [HideInInspector] public float velocityX;
    [HideInInspector] public float velocityZ;

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
    [SerializeField] public int shotsMin = 1;
    [Tooltip("How many times the enemy should shoot in a single attack (maximum value)?")]
    [SerializeField] public int shotsMax = 1;
    [Tooltip("How long should the enemy wait before shooting again in a single attack?")]
    [SerializeField] public float timeInBetweenShots = 1;
    [Tooltip("How long should the enemy wait before initiating another attack?")]
    [SerializeField] public float delayBetweenAttacks = 2;
    [Tooltip("How tall is the enemy")]
    [SerializeField] float heightLOS = 1.75f;
    [Tooltip("How fast the enemy looks at the player")]
    [SerializeField] int lookSpeed = 5;
    [Tooltip("At what percentage of HP should the enemy stagger at?")]
    [Range(0, 1)]
    [SerializeField] public float whenToStagger = 0.2f;
    [Tooltip("What are the chances of this enemy staggering while under the percentage to stagger?")]
    [Range(0, 1)]
    [SerializeField] public float chanceToStagger = 0.7f;


    //LOCAL POSITIONS AND ROTATIONS// 
    Vector3 localTargetDirection;
    Vector3 localLOSPosition;

    //ATTACK RELATED VARS//
    [Header("Attack Settings")]
    //counts how long its been since last attack
    [HideInInspector] public float timeSinceLastAttack = 0;
    [Tooltip("How much should time since last shot should effect how often this enemy attacks (multiplied)")]
    public float lastAttackMultiplier = 1;

    //has this enemy requested?
    [HideInInspector] public bool hasRequested;
    //is this enemy allowed to attack?
    [HideInInspector] public bool attackCooldownOver = true;

    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        gun = GetComponent<Gun>();
        health = GetComponent<Health>();
        ac = GameObject.Find("Attack Coordinator").GetComponent<AttackCoordinator>();
        ap = GetComponent<AttackPriority>();

        mover = GetComponent<LocomotionSimpleAgent>();
        controller = GetComponent<CharacterController>();
        localCollider = GetComponent<Collider>();
        connector = GetComponent<AgentLinkMover>();

        animator = GetComponent<Animator>();
        isMovingHash = Animator.StringToHash("isMoving");

        ragdoll = GetComponent<RagdollToggle>();

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

        if (health.gotHit)
        {
            EnterStagger();
        }

        if (health.currentHealth <= 0)
        {
            //a ludicrously high number so it doesnt pick this enemy to shoot ever again (TODO: bit of a bandaid fix, make a better solution later (maybe)
            currentState = DeadState;
            Destroy(localCollider);
            Destroy(mover);
            Destroy(ap);
            Destroy(gun);
            Destroy(controller);
            Destroy(connector);
            Destroy(agent);
            Destroy(hurtbox);

            currentState.EnterState(this);
            Destroy(this);
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
    [HideInInspector] public bool localCloseRange;
    public void CheckLocalRange()
    {
        //if range to player is greater than min & less than max
        if (localRangeToPlayer > perferedRangeMin && localRangeToPlayer < perferedRangeMax)
        {
            localInRange = true;
        }
        //if range to player is less than min
        else if(localRangeToPlayer < perferedRangeMin)
        {
            localCloseRange = true;
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


    //SHOTS WHILE MOVING 

    //NOTE: as inherrited classes are missing the monobehaviour class, they cant fire Invoke nor Coroutines. so this function is called first from those scripts 
    public void TriggerShotsWhileMoving()
    {
        StartCoroutine(FireShotsWhileMoving());
    }

    IEnumerator FireShotsWhileMoving()
    {
        int randomShots;
        randomShots = Random.Range(shotsMin, shotsMax);
        //TODO: Should set this to false AFTER it fires all of its shots
        attackCooldownOver = false;
        for (int i = 0; i < randomShots; i++)
        {
            ShootState.EnterState(this);
            yield return new WaitForSeconds(timeInBetweenShots);
        }
    }

    //STAGGER
    [HideInInspector] public bool isStaggered;
    public void EnterStagger()
    {
        if (health.currentHealth < health.maxHealth * whenToStagger)
        {
            float random = Random.Range(0f, 1f);
            if (random <= chanceToStagger && !isStaggered)
            {
                StopAllCoroutines();
                isStaggered = true;
                StaggerState.EnterState(this);
                currentState = StaggerState;
            }
        }
    }

    public void ExitStagger()
    {
        currentState = RepositionState;
        isStaggered = false;
        agent.isStopped = false;
    }

}
