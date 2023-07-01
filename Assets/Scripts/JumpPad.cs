using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("How much force will be applied to character controllers on contact with the jump pad")]
    private float padStrength;

    //Script variables
    private Vector3 triggerSize;

    private void Start() 
    {
        triggerSize = transform.localScale;
    }

    private void Update() 
    {
        Collider[] objectsInTrigger = Physics.OverlapBox(transform.position, triggerSize, Quaternion.identity);
        foreach(Collider nearbyObjects in objectsInTrigger)
        {
            if(nearbyObjects.gameObject.CompareTag("Hurtbox"))
            {
                Movement movementScript = nearbyObjects.transform.parent.gameObject.GetComponent<Movement>();
                if(movementScript)
                {
                    movementScript.isGroundPounding = false;
                    if(movementScript.currentJumps <= 0) movementScript.currentJumps ++;
                    movementScript.yVelocity = padStrength;
                }
            }
        }
    }
}
