using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class Movement : MonoBehaviour
{
    //In-editor tools 
    
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

    [Header ("Slide setting")]

    [SerializeField]
    [Tooltip ("How fast player goes during a slide")]
    private float slideSpeed;
    [SerializeField]
    [Tooltip ("Vertical position the camera will take during a slide")]
    private float slidingCameraHeight;
    [SerializeField]
    [Tooltip ("Jump out of slide properties (x: horizontal force, y: vertical force)")]
    private Vector2 longJumpStrength;

    [SerializeField]
    [Tooltip ("How long the long jump force horizontal force will be applied for")]
    private float longJumpLength;

    [Header ("Gravity Settings")]
    [SerializeField] [Tooltip ("-1 to go down, 0 for no gravity, 1 to go up")] [Range (-1, 1)] private int gravity;
    [SerializeField] [Tooltip ("How strong is the gravity")] private float gravityScale;

    //Private script variables
    [HideInInspector] public Vector2 horizontalVelocity; //Gets fed into ApplyMovement() to determine horizontal direction
    [HideInInspector] public float yVelocity; //Tracks vertical speed
    [HideInInspector] public Vector3 moveDirection; //Makes sure direction is always camera dependent 
    [HideInInspector] public int currentJumps; // Amount of jumps available to the player at any given time
    [HideInInspector] public int currentStamina; // Amount of dashes available to the player at any given time
    [HideInInspector] public bool staminaCooldownDone = true;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isGroundPounding = false;
    [HideInInspector] public bool isSliding = false;
    private bool isLongJumping = false;

    //Used to calculate force applied on ground pound bounce
    private Vector3 groundPoundStartPosition;
    private Vector3 groundPoundEndPosition;

    //Required components
    private CharacterController characterController; 
    private Transform ceilingCheck;
    private ForceReceiver forceReceiver;
    private Transform playerCamera;

    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
        currentStamina = maxStamina;
        ceilingCheck = transform.Find("Ceiling Check");
        ChangeStats();
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

            //forceReceiver.impact.y = 0;
        }

        //Removes one jump when leaving the ground
        if(!characterController.isGrounded && currentJumps == amountOfJumps)
        {
            currentJumps --;
        }

//        Debug.Log(isSliding);
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
        else if(isSliding)
        {
            characterController.Move(new Vector3(moveDirection.x * slideSpeed, yVelocity * speed, moveDirection.z * slideSpeed) * Time.deltaTime);
            return;
        }
        else if(isLongJumping)
        {
            characterController.Move(new Vector3(moveDirection.x * longJumpStrength.x, yVelocity * speed, moveDirection.z * longJumpStrength.x) * Time.deltaTime);
        }
        moveDirection = transform.right * horizontalVelocity.x + transform.forward * horizontalVelocity.y;
        moveDirection = new Vector3(moveDirection.x, yVelocity, moveDirection.z);   

        //Adds explosion force if necessary
        if(forceReceiver.receivedExplosion)
        {
            moveDirection += forceReceiver.impact;
        }

        characterController.Move(moveDirection * speed * Time.deltaTime);
    }

    public void Jump()
    {
        if(!characterController.isGrounded && currentJumps == 0) return;
        currentJumps --;
        isGroundPounding = false;

        if(isSliding)
        {
            StartCoroutine("LongJump");
            return;
        }
        yVelocity = jumpHeight;
    }

    public IEnumerator Dash()
    {
        if(currentStamina == 0 || isDashing || horizontalVelocity.magnitude < 0.1f) yield break;
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

    public void GroundPound()
    {
        if(currentStamina == 0 || isGroundPounding || characterController.isGrounded) return;

        isGroundPounding = true;
        isDashing = false;
        currentStamina --;
        groundPoundStartPosition = transform.position;
        yVelocity = groundPoundStrength;
        forceReceiver.impact.y = 0;

        StartCoroutine("RecoverStamina");
    }

    public void Slide()
    {
        if(horizontalVelocity.magnitude < 0.1f || isDashing || isSliding) return;
        playerCamera.position = new Vector3 (playerCamera.position.x, playerCamera.position.y - slidingCameraHeight, playerCamera.position.z);
        isSliding = true;
        Debug.Log("Slid");
    }

    public void CancelSlide()
    {
        if(!isSliding) return;
        isSliding = false;
        playerCamera.position = new Vector3 (playerCamera.position.x, playerCamera.position.y + slidingCameraHeight, playerCamera.position.z);
    }

    private IEnumerator LongJump()
    {
        if(!characterController.isGrounded)
        {
            CancelSlide();
            currentJumps ++;
            Jump();
            yield break;
        }

        CancelSlide();
        isLongJumping = true;
        yVelocity = longJumpStrength.y;
        horizontalVelocity += new Vector2(horizontalVelocity.x * longJumpStrength.x, horizontalVelocity.y * longJumpStrength.x);

        yield return new WaitForSeconds(longJumpLength);
        isLongJumping = false;
    }

    public void GroundPoundBounce()
    {
        groundPoundEndPosition = transform.position;
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
        playerCamera = GetComponentInChildren<CameraMove>().gameObject.transform;
        Debug.Log(playerCamera.name);
        ceilingCheck.position = playerCamera.position;
    }
}
