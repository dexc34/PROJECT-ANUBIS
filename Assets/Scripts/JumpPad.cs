using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("How much force will be applied to character controllers on contact with the jump pad")]
    private float padStrength;

    private void OnTriggerEnter(Collider other) 
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if(playerMovement)    
        {
            playerMovement.yVelocity = padStrength;
        }
    }
}
