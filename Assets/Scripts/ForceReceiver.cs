using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReceiver : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("Defines how heavy the object is")]
    private float mass = 3; 

    [SerializeField]
    [Tooltip ("How long it takes for the explosion force to go away")]
    private float explosionForceDuration = 5;

    //Script Variables
    [HideInInspector] public Vector3 impact = Vector3.zero;
    [HideInInspector] public bool receivedExplosion;
    private float lerpMultiplier;

    //Required components
    private CharacterController characterController;
    private PlayerMovement playerMovement;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        lerpMultiplier = 100/(explosionForceDuration * 100);
        Debug.Log(lerpMultiplier);
    }

    // call this function to add an impact force:
    public void ReceiveExplosion(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
        playerMovement.yVelocity = 0;
        impact += direction.normalized * force / mass;
    }

    private void Update()
    {
        // Change bool which allows player movement to account for the impact
        if (impact.magnitude > 0.2f) receivedExplosion = true;
        else receivedExplosion = false;

        // Adds drag to the explosion
        impact = Vector3.Lerp(impact, Vector3.zero, lerpMultiplier*Time.deltaTime);
    }
}
