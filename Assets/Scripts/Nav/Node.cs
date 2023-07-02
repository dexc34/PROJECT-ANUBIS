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
    [HideInInspector] public bool isUsed;
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
        CheckIfUsed();
    }

    //Calculate LOS to player
    void CalculateLOS()
    {
        targetDirection = (player.transform.position - LOSPosition).normalized;
        if (Physics.Raycast(LOSPosition, targetDirection, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != player)
            {
                Debug.DrawRay(LOSPosition, targetDirection * hit.distance, Color.red);
                Gizmos.color = Color.red;
                isValid = false;
            }
            else if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawRay(LOSPosition, targetDirection * hit.distance, Color.green);
                Gizmos.color = Color.green;
                isValid = true;
            }
            else
            {
                Gizmos.color = Color.red;
                isValid = false;
            }
        }
    }

    public void CheckIfUsed()
    {
        if (Physics.CheckSphere(transform.position, 1f, enemy))
        {
            Gizmos.color = Color.yellow;
            isUsed = true;
        }
        else
        {
            isUsed = false;
        }
    }

    public bool IsUsed(NavMeshAgent agent)
    {
        return isUsed;
    }
}
