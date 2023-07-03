using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    GameObject player;
    RaycastHit hit;

    Vector3 LOSPosition;
    Vector3 targetDirection;

    [SerializeField] LayerMask enemy;

    [HideInInspector] public bool isValid;
    [HideInInspector] public bool inRange;
    [HideInInspector] public bool inLOS;

    [HideInInspector] public float rangeToPlayer;

    public bool hasReached;
    //draw
    void OnDrawGizmosSelected()
    {
        //draws a green cube where the node is 
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(0.25f, 0.25f, 0.25f));
    }

    private void Start()
    {
        player = GameObject.Find("Player");

        LOSPosition = transform.position + new Vector3(0,1.75f,0);
    }

    private void Update()
    {
        CalculateLOS();
        CalculateRange();
        CheckIfValid();
    }

    //Calculate LOS to player
    void CalculateLOS()
    {
        targetDirection = (player.transform.position - LOSPosition).normalized;
        if (Physics.Raycast(LOSPosition, targetDirection, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != player)
            {
                inLOS = false;
            }
            else if (hit.collider.gameObject.CompareTag("Player"))
            {
                inLOS = true;
            }
            else
            {
                inLOS = false;
            }
        }
    }

    void CalculateRange()
    {
        rangeToPlayer = hit.distance;
    }

    public void CheckRange(float minRange, float maxRange)
    {
        if(rangeToPlayer > minRange && rangeToPlayer < maxRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }

    void CheckIfValid()
    {
        if (inLOS && inRange)
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hackable"))
        {
            hasReached = true;
        }
        else
        {
            hasReached = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hurtbox"))
        {
            hasReached = false;
        }
    }

}
