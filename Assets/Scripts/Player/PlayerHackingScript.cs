using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerHackingScript : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip("The colour enemies will be highlighted when looking at them")]
    private Color highlightColour = Color.red;

    [SerializeField]
    [Tooltip("The colour enemies will be highlighted when hacking them")]
    private Color hackingColour = Color.green;

    [SerializeField]
    [Tooltip("Default Colour of the tether line that appears when you're hacking an enemy")]
    private Color defaultTetherColour = Color.white;

    [SerializeField]
    [Tooltip("Colour that things turn when the line of sight of a hack is interrupted")]
    private Color hackInterruptionColour = Color.yellow;

    [SerializeField]
    [Tooltip("How wide the outline \n" + "will be when the \n" + "player looks at them\n")]
    private float highlightWidth = 7;

    [SerializeField]
    [Tooltip("The temporary value for how long it takes to hack an enemy (change later)")]
    public float tempHackingDurration;

    [SerializeField]
    [Tooltip("How long a hack can be interrupted for before the hack is canceled")]
    public float allowedHackingInterruption = 1;

    [SerializeField]
    [Tooltip("The length of time you are moving between your current position and the robot you hacked")]
    public float hackMoveDurration = 0.5f;

    [SerializeField]
    [Tooltip("Player Camera prefab goes here")]
    private GameObject cameraPrefab;

    [SerializeField] private LayerMask ignoredLayer;

    //Required components
    private Cinemachine.CinemachineBrain mainCameraBrain;

    [HideInInspector]
    public GameObject newCamera;

    private GameObject currentlySelectedEnemy = null;
    private Outline selectedEnemiesOutline = null;

    private GameObject currentlyHackingEnemy = null;
    private Outline hackingEnemiesOutline = null;

    private GameObject currentlyStoredEnemy = null;

    private CharacterController characterController;
    private LineRenderer lineRenderer;
    private Movement playerMovementScript;
    private Gun gunScript;
    private Melee meleeScript;
    private Health healthScript;
    private SecondaryAbility secondaryAbilityScript;

    //Script variables
    private float raycastDistance = Mathf.Infinity;
    [HideInInspector] public bool hackInputDetected = false;
    public float hackingTimer = -1000;
    public float hackingInterruptionTimer = -1000;

    [HideInInspector] public bool hacking = false;
    [HideInInspector] public bool hackInterrupted = false;
    [HideInInspector] public bool transitioningBetweenEnemies = false;
    float distanceToEnemy;
    float moveStep;
    Vector3 directionToEnemy;
    GameObject playerCamera;

    private bool enemyHasShield = false;
    [HideInInspector] public bool needSecondInput = false;



    //Built in stuff
    //##############################################################################################
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        lineRenderer = GetComponent<LineRenderer>();
        mainCameraBrain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Cinemachine.CinemachineBrain>();
        playerCamera = transform.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().gameObject;
        playerMovementScript = GetComponent<Movement>();
        gunScript = GetComponent<Gun>();
        meleeScript = GetComponent<Melee>();
        healthScript = GetComponent<Health>();
        secondaryAbilityScript = GetComponent<SecondaryAbility>();
    }

    void Update()
    {
//        Debug.Log("Move Step: " +  moveStep);


        //sets the first point of the hacking tether line to always be centered on the player
        lineRenderer.SetPosition(0, transform.position);

        if (hacking && !hackInputDetected)
            ExitHackMode();

        if (hacking)
        {
            //sets the second point of the hacking tether line to be at the location of the enemy
            lineRenderer.SetPosition(1, currentlyHackingEnemy.transform.position + new Vector3(0, 1, 0));
            HackingTetherCheckRaycast();
        }

        else if (!hacking)
        {
            //sets the second point of the hacking tether line back to the location of the player
            lineRenderer.SetPosition(1, transform.position);
            EnemyDetectionRaycast();
        }

        RunTimer();
    }

    private void FixedUpdate()
    {
        //makes the player move in the direction of the enemy that they are hacking
        if (transitioningBetweenEnemies)
        {
            characterController.Move(directionToEnemy * moveStep * Time.deltaTime);
        }

    }

    //The Raycasting
    //##############################################################################################
    public void EnemyDetectionRaycast()
    {
        //stores the data of the object that has been hit by the raycast
        RaycastHit hit;

        //the start and direction of the raycast (the location of the camera and which direction its pointing)
        Vector3 raycastStart = Camera.main.transform.position;
        Vector3 raycastDirection = Camera.main.transform.TransformDirection(Vector3.forward);



        //runs the code inside if a raycast from raycastStart to raycastDistance pointed in the direction of racastDirection hits anything with a collider
        if (Physics.Raycast(raycastStart, raycastDirection, out hit, raycastDistance))
        {
            //checks to see if the object currently in the raycast is the enemy that is currently being stored
            if (hit.collider.gameObject != currentlySelectedEnemy)
            {
                UnstoreEnemy();
            }


            //runs if the object hit with the raycast has the Hackable tag
            if (hit.collider.gameObject.CompareTag("Hackable"))
            {
                if (currentlySelectedEnemy == null)
                {
                    //stores the enemy's game object and also adds an outline to it
                    currentlySelectedEnemy = hit.collider.gameObject;
                    selectedEnemiesOutline = currentlySelectedEnemy.AddComponent<Outline>();

                    //turns on the outline of the selected enemy, sets it's colour and it's width
                    if (selectedEnemiesOutline != null)
                    {
                        selectedEnemiesOutline.enabled = true;
                        selectedEnemiesOutline.OutlineColor = highlightColour;
                        selectedEnemiesOutline.OutlineWidth = highlightWidth;
                    }

                }

                //some debug stuff we can get rid of later
                Debug.DrawRay(raycastStart, raycastDirection * hit.distance, Color.yellow);

                if (!hacking && hackInputDetected)
                {
                    currentlyHackingEnemy = currentlySelectedEnemy;
                    hackingEnemiesOutline = currentlyHackingEnemy.GetComponent<Outline>();
                    hackingEnemiesOutline.OutlineColor = hackingColour;
                    hackInterrupted = false;
                    StartHackingTimer(tempHackingDurration);
                    enemyHasShield = false;
                    hacking = true;

                    //Can't hack enemy if they've got an antivirus shield
                    if (currentlyHackingEnemy.GetComponent<Health>().hasAntivirusShield)
                    {
                        enemyHasShield = true;
                        InterruptHack();
                    }
                }
            }
        }
        else
        {
            UnstoreEnemy();

            Debug.DrawRay(raycastStart, raycastDirection * 1000, Color.white);
        }
    }

    private void UnstoreEnemy()
    {
        //deletes the enemy's outline and unstores the gameobject and outline
        if (currentlySelectedEnemy != null)
        {
            if (currentlySelectedEnemy != currentlyHackingEnemy)
                Destroy(currentlySelectedEnemy.GetComponent<Outline>());

            selectedEnemiesOutline = null;
            currentlySelectedEnemy = null;
        }
    }

    public void HackingTetherCheckRaycast()
    {
        if (enemyHasShield) return;
        //stores the data of the object that has been hit by the raycast
        RaycastHit hit;

        //the start and direction of the raycast (the location of the camera and the direction to the enemy)
        Vector3 raycastStart = transform.position;
        Vector3 raycastDirection = (currentlyHackingEnemy.transform.position + new Vector3(0, 1, 0) - transform.position);

        if (Physics.Raycast(raycastStart, raycastDirection, out hit, raycastDistance, ignoredLayer))
        {
            if (hit.collider.gameObject == currentlyHackingEnemy)
            {
                lineRenderer.startColor = defaultTetherColour;
                lineRenderer.endColor = lineRenderer.startColor;
                hackingEnemiesOutline.OutlineColor = hackingColour;

                StartHackInterruptionTimer(allowedHackingInterruption);

                hackInterrupted = false;
            }
            else
            {
                InterruptHack();
            }
        }
    }

    private void InterruptHack()
    {
        if (!hackInterrupted)
        {
            StartHackInterruptionTimer(allowedHackingInterruption);
            lineRenderer.startColor = hackInterruptionColour;
            lineRenderer.endColor = lineRenderer.startColor;
            hackingEnemiesOutline.OutlineColor = hackInterruptionColour;
            hackInterrupted = true;
        }
    }

    //Timer Stuff
    //##############################################################################################
    public void StartHackingTimer(float timerDurration)
    {
        hackingTimer = timerDurration;
    }

    public void StartHackInterruptionTimer(float timerDurration)
    {
        hackingInterruptionTimer = timerDurration;
    }


    private void RunTimer()
    {
        if (hackingTimer > 0 && !hackInterrupted)
        {
            hackingTimer -= Time.deltaTime;
        }
        else if (hackingTimer < 0.1f && hackingTimer > -0.1f)
        {
            StartCoroutine("HackEnemy");
        }

        if (hackingInterruptionTimer > 0)
        {
            hackingInterruptionTimer -= Time.deltaTime;
        }
        else if (hackingInterruptionTimer < 0.1f && hackingInterruptionTimer > -0.1f)
        {
            ExitHackMode();
        }
    }




    //Hacking Stuff
    //##############################################################################################
    public void ExitHackMode()
    {
        hacking = false;
        hackInputDetected = false;
        needSecondInput = true;

        //resets the timer to a very low number so that it doesn't do anything (see RunTimer())
        hackingTimer = hackingInterruptionTimer = -1000;

        //gets rid of the outline of the enemy and then sets the
        //variables that store the game object and the outline to
        //nothing
        hackingEnemiesOutline.OutlineColor = highlightColour;

        if (!transitioningBetweenEnemies)
        {
            hackingEnemiesOutline = null;
            currentlyHackingEnemy = null;
        }

    }

    private IEnumerator HackEnemy()
    {
        //saves the location of the player to put the stored player when it is released
        Vector3 playerOriginPoint = transform.position;

        //turns off all collisions for the player (idk if this even works)
        for (int i = 0; i <= 31; i++)
        {
            Physics.IgnoreLayerCollision(0, i, true);
        }

        //turns off a bunch of stuff in the enememy so that it stops moving and doesn't interact with the player
        if (currentlyHackingEnemy.GetComponent<GruntStateMachine>() && currentlyHackingEnemy.GetComponent<NavMeshAgent>())
        {
            currentlyHackingEnemy.GetComponent<NavMeshAgent>().enabled= false;
            currentlyHackingEnemy.GetComponent<GruntStateMachine>().enabled = false;
        }
        currentlyHackingEnemy.GetComponent<CapsuleCollider>().enabled= false;



        //turns on the transitioning boolean, takes away the player's
        //ability to move, and turns off collisions so that the player
        //doesn't collide with things along the way
        transitioningBetweenEnemies = true;
        playerMovementScript.canMove = false;

        
        //calculates the angle to the enemy (normalized to a magnitude of 1), how far it is, and how fast it can get there
        directionToEnemy = (currentlyHackingEnemy.transform.Find("Camera Spawn Point").transform.position - transform.position).normalized;
        distanceToEnemy = Vector3.Distance(transform.position, currentlyHackingEnemy.transform.position);
        moveStep = distanceToEnemy / hackMoveDurration;


        ExitHackMode();


        //Waits until the the transition (or camera blend) is over to continue the code, this parameter should be dynamic in the future
        yield return new WaitForSeconds(hackMoveDurration);


        //turns off the currently stored enemy and makes it the child of the player to be released later
        currentlyHackingEnemy.SetActive(false);
        currentlyHackingEnemy.transform.parent = transform;


        //turns the stuff in the enemy back on
        if (currentlyStoredEnemy != null)
        {
            if (currentlyStoredEnemy.GetComponent<GruntStateMachine>() && currentlyStoredEnemy.GetComponent<NavMeshAgent>() )
            {
                currentlyStoredEnemy.GetComponent<GruntStateMachine>().enabled = true;
                currentlyStoredEnemy.GetComponent<NavMeshAgent>().enabled= true;
            }
            currentlyStoredEnemy.GetComponent<CapsuleCollider>().enabled = true;
        }


        //turns off the transitioning boolean and gives back the player's ability to move
        transitioningBetweenEnemies = false;
        playerMovementScript.canMove = true;


        //releases the currently stored enemy if there is one
        if (currentlyStoredEnemy != null)
        {
            //sets the enemy back to being it's own parent, teleports it to where the player was when they started transitioning and turns it back on
            currentlyStoredEnemy.transform.parent = null;
            currentlyStoredEnemy.transform.position = playerOriginPoint;
            currentlyStoredEnemy.SetActive(true);
            currentlyStoredEnemy.GetComponent<Gun>().UpdateGunStats(gunScript);
            currentlyStoredEnemy.GetComponent<Health>().UpdateHealth(healthScript);
            Destroy(currentlyStoredEnemy.GetComponent<Outline>());
        }

        //stores the currently hacking enemy as a variable
        currentlyStoredEnemy = currentlyHackingEnemy;
        currentlyHackingEnemy = null;


        //turns the player's collisions back on
        for (int i = 0; i <= 31; i++)
        {
            Physics.IgnoreLayerCollision(0, i, false);
        }

        //Update player stats
        playerMovementScript.ChangeStats();
        gunScript.UpdateGunStats(currentlyStoredEnemy.GetComponent<Gun>());
        meleeScript.UpdateMelee(currentlyStoredEnemy.GetComponent<Melee>());
        healthScript.UpdateHealth(currentlyStoredEnemy.GetComponent<Health>());
        secondaryAbilityScript.UpdateSecondary(currentlyStoredEnemy.GetComponent<SecondaryAbility>());
    }
}
