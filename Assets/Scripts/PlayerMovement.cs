using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    //In-editor tools 
    [SerializeField] private InputAction move;
    [SerializeField] private float speed;

    [Header ("Jump Settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private int amountOfJumps = 2;

    [Header ("Stamina Settings")]
    [SerializeField] [Tooltip ("How much max stamina the player has")] public int maxStamina = 2;
    [SerializeField] [Tooltip("How a stamina bar takes to refill in seconds")] public float staminaCooldown;


    [Header ("Dash Settings")]
    [SerializeField] [Tooltip ("How fast does the player become while dashing")]private float dashSpeed;
    [SerializeField] [Tooltip ("How long does the dash go for in seconds")] public float dashDuration;

    [Header ("Ground Pound Settings")]

    [SerializeField]
    [Tooltip ("How strongly the player gets pulled down to the ground")]
    private float groundPoundStrength;

    [SerializeField]
    [Tooltip ("How much of the vertical momentum is maintained when bouncing up (distance fell / fractionOfMomentumPreserved)")]
    private float fractionOfMomentumPreserved;

    [SerializeField]
    [Tooltip ("Min and maximum height the ground pound bounce will have")]
    private Vector2 groundPoundBounceLimit;

    [Header ("Gravity Settings")]
    [SerializeField] [Tooltip ("-1 to go down, 0 for no gravity, 1 to go up")] [Range (-1, 1)] private int gravity;
    [SerializeField] [Tooltip ("How strong is the gravity")] private float gravityScale;

    //Private script variables
    [HideInInspector] public float yVelocity; //Tracks vertical speed
    [HideInInspector] public Vector3 moveDirection; //Makes sure direction is always camera dependent 
    [HideInInspector] public int currentJumps; // Amount of jumps available to the player at any given time
    [HideInInspector] public int currentStamina; // Amount of dashes available to the player at any given time
    [HideInInspector] public bool staminaCooldownDone = true;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isGroundPounding = false;
    private Vector3 groundPoundStartPosition;
    private Vector3 groundPoundEndPosition;

    //Required components
    private CharacterController characterController; 
    private Transform ceilingCheck;
    private ForceReceiver forceReceiver;

    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
        ceilingCheck = transform.Find("Ceiling Check");
        ChangeStats();
        currentStamina = maxStamina;
        move.Enable();
    }

    private void Update() 
    {
        //Reload scene when falling off a certain value, only for debugging
        if(transform.position.y < -7)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //Remove all upwards momentum when hiting head on ceiling
        if(Physics.CheckBox(ceilingCheck.position, new Vector3(.1f, 1, .1f), transform.rotation, 7))
        {
            yVelocity = -.2f;
            forceReceiver.impact.y = 0;
        }

        ApplyGravity();
        ApplyMovement();
        
        if(characterController.isGrounded)
        {
            //Restores jumps on landing
            if(currentJumps != amountOfJumps)
            {
                currentJumps = amountOfJumps;
            }

            if(isGroundPounding)
            {
                isGroundPounding = false;
                groundPoundEndPosition = transform.position;
                GroundPoundBounce();
            }

            //forceReceiver.impact.y = 0;
        }

        //Removes one jump when leaving the ground
        if(!characterController.isGrounded && currentJumps == amountOfJumps)
        {
            currentJumps --;
        }
    }

    private void ApplyGravity()
    {
        if(isDashing || forceReceiver.receivedExplosion) return;
        //No gravity gets applied if grounded
        if (characterController.isGrounded && yVelocity < 0)
        {
            yVelocity = -0.5f;
        }
        //Apply gravity when not grounded
        else
        {
            yVelocity += gravity * gravityScale * Time.deltaTime;
        }
        moveDirection.y = yVelocity;
    }

    private void ApplyMovement()
    {
        if(isDashing)
        {
            characterController.Move(moveDirection * dashSpeed * Time.deltaTime);
            return;
        }
        Vector2 inputReadings = move.ReadValue<Vector2>();
        moveDirection = transform.right * inputReadings.x + transform.forward * inputReadings.y;
        moveDirection = new Vector3(moveDirection.x, yVelocity, moveDirection.z);   

        //Adds explosion force if necessary
        if(forceReceiver.receivedExplosion)
        {
            moveDirection += forceReceiver.impact;
        }

        characterController.Move(moveDirection * speed * Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!characterController.isGrounded && currentJumps == 0) return;
        if(context.performed)
        {
            yVelocity = jumpHeight;
            currentJumps --;
            isGroundPounding = false;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(currentStamina == 0 || !context.performed || isDashing) return;
        StartCoroutine("Dashing");
    }

    public IEnumerator Dashing()
    {
        currentStamina --;
        yVelocity = 0;
        forceReceiver.impact.y = 0;
        moveDirection.y = yVelocity;
        isDashing = true;
        isGroundPounding = false;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        StartCoroutine("RecoverStamina");
    }

    public void GroundPound(InputAction.CallbackContext context)
    {
        if(currentStamina == 0 || !context.performed || isGroundPounding || characterController.isGrounded) return;

        isGroundPounding = true;
        isDashing = false;
        currentStamina --;
        groundPoundStartPosition = transform.position;
        yVelocity = groundPoundStrength;
        forceReceiver.impact.y = 0;

        StartCoroutine("RecoverStamina");
    }

    private void GroundPoundBounce()
    {
        float distanceFell = groundPoundStartPosition.y - groundPoundEndPosition.y;
        float groundPoundBounceStrength = distanceFell/fractionOfMomentumPreserved;
        groundPoundBounceStrength = Mathf.Clamp(groundPoundBounceStrength, groundPoundBounceLimit.x, groundPoundBounceLimit.y);
        yVelocity = groundPoundBounceStrength;
    }

    public IEnumerator RecoverStamina()
    {
        //Dont start cooldown timer until the previous one is done
        while(!staminaCooldownDone) yield return null;
    
        //Return ability to dash after cooldown
        staminaCooldownDone = false;

        yield return new WaitForSeconds(staminaCooldown);
        staminaCooldownDone = true;
        currentStamina ++;
    }

    public void ChangeStats()
    {
        ceilingCheck.position = transform.Find("Player Camera").position;
    }
}
