using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloserToThePrey : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    public float cooldown = 6;

    [SerializeField]
    private float range = 30;

    [SerializeField]
    [Tooltip("How long in seconds it will take for the blade to reach the end of the range")]
    private float timeToReachMaxRange = .5f;

    [SerializeField]
    [Tooltip("How long in seconds it will take for the player to be pulled towards the target")]
    private float pullTime = .3f;

    [SerializeField]
    [Tooltip("How long in seconds is the delay before you start getting pulled towards target upon hitting them")]
    private float pullDelay = .1f;

    [SerializeField]
    private LayerMask layersToIgnore;

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
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<Movement>();
        layersToIgnore |= (1 << 0);
        layersToIgnore |= (1 << 1);
        layersToIgnore |= (1 << 2);
        layersToIgnore |= (1 << 4);
        layersToIgnore |= (1 << 5);
        layersToIgnore |= (1 << 6);
        layersToIgnore |= (1 << 7);
        layersToIgnore |= (1 << 8);
        layersToIgnore |= (1 << 10);
        layersToIgnore |= (1 << 11);
        layersToIgnore |= (1 << 12);
        layersToIgnore |= (1 << 14);
        layersToIgnore |= (1 << 15);
        if(transform.CompareTag("Player")) 
        {
            layersToIgnore &= ~(1 << 7);
        }
    }

    private void Update()
    {
        //Raycast
        if (abilityInUse)
        {
            currentDistance += Time.deltaTime * (range / timeToReachMaxRange);

            throwRaycast = new Ray(originPoint.transform.position, originPoint.transform.forward);
            Debug.DrawLine(originPoint.transform.position, throwRaycast.GetPoint(currentDistance), Color.red);

            if (Physics.Raycast(throwRaycast, out RaycastHit hitInfo, currentDistance, layersToIgnore))
            {
                Debug.Log("Object hit: " + hitInfo + ", Layer: " + hitInfo.transform.gameObject.layer);
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
        movementScript.yVelocity = 0;
        movementScript.canMove = true;
    }

    private IEnumerator BeginPull()
    {
        movementScript.canMove = false;
        movementScript.CancelAllActions();
        abilityInUse = false;
        currentDistance = 0;
        moveDirection = (targetPosition - transform.position).normalized;
        pullForce = (targetPosition - transform.position).magnitude / pullTime;
        StartCoroutine("CancelPull");

        yield return new WaitForSeconds(pullDelay);

        isGettingPulled = true;
    }

    public void UseCloserToThePrey(Transform origin)
    {
        currentDistance = 0;
        originPoint =  new GameObject("Closer to the prey origin"); 
        originPoint.transform.position = origin.position;
        originPoint.transform.rotation = origin.rotation;
        moveDirection = Vector3.zero;
        abilityInUse = true;
    }

    private void OnDestroy() 
    {
        Destroy(originPoint);
    }

}
