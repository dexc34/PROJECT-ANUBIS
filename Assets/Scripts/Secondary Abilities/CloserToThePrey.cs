using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloserToThePrey : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    private bool stopAllFunctionality = false;


    [Header("Stats")]
    public float cooldown;

    [SerializeField]
    [Tooltip("How long it takes for the ability to become available after being used")]
    private float range;

    [SerializeField]
    [Tooltip("How long in seconds it will take for the blade to reach the end of the range")]
    private float timeToReachMaxRange;

    [SerializeField]
    [Tooltip("How long in seconds it will take for the player to be pulled towards the target")]
    private float pullTime;

    [SerializeField]
    [Tooltip("How long in seconds is the delay before you start getting pulled towards target upon hitting them")]
    private float pullDelay;

    [SerializeField]
    [Tooltip("How long in seconds it takes for the player to start moving after reaching their target")]
    private float pullCancelDelay;

    [SerializeField]
    [Tooltip("What layers does the raycast check for")]
    private LayerMask layersToCheck;

    //Script variables
    private bool abilityInUse = false;
    private float pullForce;
    private bool isGettingPulled = false;
    private float currentDistance;
    Vector3 moveDirection;
    Vector3 targetPosition;

    //Required components
    private GameObject originPoint;
    private Movement movementScript;
    private CharacterController characterController;
    private Ray throwRaycast;

    private void Start()
    {
        if(stopAllFunctionality) return;

        GetStats(GameObject.Find("Secondary Ability Manager").GetComponent<CloserToThePrey>());

        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<Movement>();

        if (transform.CompareTag("Player"))
        {
            layersToCheck &= ~(1 << 7);
        }
    }

    private void Update()
    {
        if(stopAllFunctionality) return;

        //Raycast
        if (abilityInUse)
        {
            currentDistance += Time.deltaTime * (range / timeToReachMaxRange);

            throwRaycast = new Ray(originPoint.transform.position, originPoint.transform.forward);
            Debug.DrawLine(originPoint.transform.position, throwRaycast.GetPoint(currentDistance), Color.red);

            if (Physics.Raycast(throwRaycast, out RaycastHit hitInfo, currentDistance, layersToCheck))
            {
                targetPosition = throwRaycast.GetPoint(currentDistance);
                StartCoroutine("BeginPull");
            }

            if (currentDistance >= range)
            {
                Destroy(originPoint);
                abilityInUse = false;
            }
        }

        //Get pulled
        if (isGettingPulled)
        {
            characterController.Move(moveDirection * pullForce * Time.deltaTime);
        }

    }

    private IEnumerator CancelPull()
    {
        yield return new WaitForSeconds(pullTime + pullDelay);

        isGettingPulled = false;
        Destroy(originPoint);

        yield return new WaitForSeconds(pullCancelDelay);

        movementScript.yVelocity = 0;
        movementScript.canMove = true;
        movementScript.canAct = true;
    }

    private IEnumerator BeginPull()
    {
        movementScript.yVelocity = 0;
        movementScript.canMove = false;
        movementScript.canAct = false;
        movementScript.CancelAllActions();
        abilityInUse = false;
        currentDistance = 0;
        moveDirection = (targetPosition - transform.position).normalized;
        pullForce = (targetPosition - transform.position).magnitude / pullTime;
        StartCoroutine("CancelPull");
        if (movementScript.currentJumps <= 0) movementScript.currentJumps++;

        yield return new WaitForSeconds(pullDelay);

        isGettingPulled = true;
    }

    public void UseCloserToThePrey(Transform origin)
    {
        currentDistance = 0;
        originPoint = new GameObject("Closer to the prey origin");
        originPoint.transform.position = origin.position;
        originPoint.transform.rotation = origin.rotation;
        moveDirection = Vector3.zero;
        abilityInUse = true;
    }

    private void GetStats(CloserToThePrey closerToThePreyScriptToPullFrom)
    {
        cooldown = closerToThePreyScriptToPullFrom.cooldown;
        range = closerToThePreyScriptToPullFrom.range;
        timeToReachMaxRange = closerToThePreyScriptToPullFrom.timeToReachMaxRange;
        pullTime = closerToThePreyScriptToPullFrom.pullTime;
        pullDelay = closerToThePreyScriptToPullFrom.pullDelay;
        pullCancelDelay = closerToThePreyScriptToPullFrom.pullCancelDelay;
        layersToCheck = closerToThePreyScriptToPullFrom.layersToCheck;
    }

    private void OnDestroy()
    {
        Destroy(originPoint);
    }

}
