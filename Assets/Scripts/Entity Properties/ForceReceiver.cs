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
    [Tooltip ("How much horizontal force may be applied from an explosion")]
    private float horizontalForceCap;

    [SerializeField]
    [Tooltip ("How much vertical force may be applied from an explosion")]
    private float verticalForceCap;

    [SerializeField]
    [Tooltip ("How long it takes for the explosion force to go away")]
    private float explosionForceDuration = 5;

    [SerializeField]
    [Tooltip ("How horizontal explosion force will decrease over time")]
    private AnimationCurve horizontalCurve;

    [SerializeField]
    [Tooltip ("How vertical explosion force will decrease over time")]
    private AnimationCurve verticalCurve;

    //Script Variables
    [HideInInspector] public Vector3 impact = Vector3.zero;
    [HideInInspector] public bool receivedExplosion;
    private float horizontalTime;
    private float verticalTime;
    private bool degradeYMomentum = false;
    private bool yNegative = false;

    //Required components
    private CharacterController characterController;
    private Movement movementScript;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<Movement>();
    }

    // call this function to add an impact force:
    public void ReceiveExplosion(Vector3 direction, float force, bool reflectYForce)
    {
        degradeYMomentum = reflectYForce;


        movementScript.isGroundPounding = false;    
        movementScript.isDashing = false;
        movementScript.CancelSlide();

        direction.Normalize();

        if(reflectYForce)
        {
            if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
            yNegative = false;
        }

        //Only restore jump if explosion force received is positive on the y axis
        if(direction.y > 0)
        {
            if(movementScript.currentJumps <= 0) movementScript.currentJumps ++;
            yNegative = false;
        }
        else yNegative = true;
        
        movementScript.yVelocity = 0;
        verticalTime = 0;
        horizontalTime = 0;
        impact += direction.normalized * force / mass;

        //Limit how much force may be applied
        impact.x = Mathf.Clamp(impact.x, -horizontalForceCap , horizontalForceCap);
        impact.y = Mathf.Clamp(impact.y, -verticalForceCap, verticalForceCap);
        impact.z = Mathf.Clamp(impact.z, -horizontalForceCap, horizontalForceCap);
    }

    private void Update()
    {
        // Change bool which allows player movement to account for the impact
        if (impact.magnitude > 0.2f) receivedExplosion = true;
        else receivedExplosion = false;

        if(!receivedExplosion) return;

        // Adds drag to the explosion by reading the animaton curve
        impact.x = impact.x * horizontalCurve.Evaluate(horizontalTime) * explosionForceDuration;
        impact.z = impact.z * horizontalCurve.Evaluate(horizontalTime) * explosionForceDuration;
        horizontalTime += Time.deltaTime;

        if(!yNegative)
        {
            impact.y =  impact.y * verticalCurve.Evaluate(verticalTime) * explosionForceDuration;
            verticalTime += Time.deltaTime;
        }
        else
        {
            impact.y += movementScript.gravity * movementScript.gravityScale * Time.deltaTime;
            if(characterController.isGrounded)
            {
                impact = Vector3.zero;
            }
        }

    }
}
