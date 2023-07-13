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

    [SerializeField]
    [Tooltip ("How much damage the ground pound launch deals")]
    private int groundPoundLaunchDamage;

    [SerializeField]
    [Tooltip ("The radius of the ground pound launch area of effect")]
    private float groundPoundLaunchRange;

    [SerializeField]
    [Tooltip ("How high up enemies will be sent up while in range")]
    private Vector2 groundPoundLaunchStrengthRange;

    [SerializeField]
    private float rigirbodyMultipier;

    [Header ("Slide setting")]

    [SerializeField]
    [Tooltip ("How fast player goes during a slide")]
    private float slideSpeed;

    [SerializeField]
    [Tooltip ("How slow a player's slide must be for it to be canceled (used when hitting walls)")]
    private float slideLeeway = 5;

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
    [SerializeField] [Tooltip ("-1 to go down, 0 for no gravity, 1 to go up")] [Range (-1, 1)] public int gravity;
    [SerializeField] [Tooltip ("How strong is the gravity")] public float gravityScale;

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
    [HideInInspector] public bool canMove = true;

    //Used to calculate force applied on ground pound bounce
    private Vector3 groundPoundStartPosition;
    private Vector3 groundPoundEndPosition;

    //Required components
    private CharacterController characterController; 
    private Transform ceilingCheck;
    private ForceReceiver forceReceiver;
    private Transform playerCamera;
    private PlayerAudio playerAudio;

    //TODO: Implement these. currently after hacking these get destroyed, so we have to instantiate new particles for these
    //PARTICLES 
    //[Header("Particles")]
    //public ParticleSystem slideParticle;

    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
        currentStamina = maxStamina;
        ceilingCheck = transform.Find("Ceiling Check");
        playerAudio = GetComponentInChildren<PlayerAudio>();
        currentJumps = amountOfJumps;

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
        
        //Removes one jump when leaving the ground
        if(!characterController.isGrounded && currentJumps == amountOfJumps)
        {
            currentJumps --;
        }

        if(characterController.isGrounded && currentJumps != amountOfJumps)
        {
            currentJumps = amountOfJumps;
        }
    }

    private void ApplyGravity()
    {
        if(isDashing || forceReceiver.receivedExplosion) return;
        //No gravity gets applied if grounded
        if (characterController.isGrounded && yVelocity < 0)
        {
            yVelocity = -0.5f;
            if(isSliding) yVelocity = -1;
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
        if(!canMove) return;
        if(isDashing)
        {
            characterController.Move(moveDirection * dashSpeed * Time.deltaTime);
            return;
        }
        else if(isSliding)
        {
            characterController.Move(new Vector3(moveDirection.x * slideSpeed, yVelocity * speed, moveDirection.z * slideSpeed) * Time.deltaTime);
            Vector2 slideXZSpeed =  new Vector2(characterController.velocity.x, characterController.velocity.z);
            if(slideXZSpeed.magnitude < slideLeeway) CancelSlide();
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
        if(currentJumps <= 0 || !canMove) return;
        currentJumps --;
        isGroundPounding = false;

        if(isSliding)
        {
            StartCoroutine("LongJump");
            return;
        }
        playerAudio.PlayJumpSFX();
        yVelocity = jumpHeight;
    }

    public IEnumerator Dash()
    {
        if(currentStamina == 0 || isDashing || horizontalVelocity.magnitude < 0.1f || !canMove) yield break;
        currentStamina --;
        yVelocity = 0;
        forceReceiver.impact.y = 0;
        moveDirection.y = yVelocity;
        playerAudio.PlayDashSFX();
        isDashing = true;
        isGroundPounding = false;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        StartCoroutine("RecoverStamina");
    }

    public void GroundPound()
    {
        if(currentStamina == 0 || isGroundPounding || characterController.isGrounded || !canMove) return;

        isGroundPounding = true;
        isDashing = false;
        currentStamina --;
        groundPoundStartPosition = transform.position;
        yVelocity = groundPoundStrength;
        forceReceiver.impact.y = 0;

        StartCoroutine("RecoverStamina");
    }

    public void GroundPoundBounce()
    {
        groundPoundEndPosition = transform.position;
        float distanceFell = groundPoundStartPosition.y - groundPoundEndPosition.y;
        float groundPoundBounceStrength = distanceFell/fractionOfMomentumPreserved;
        GroundPoundLaunch();
        groundPoundBounceStrength = Mathf.Clamp(groundPoundBounceStrength, groundPoundBounceLimit.x, groundPoundBounceLimit.y);
        yVelocity = groundPoundBounceStrength;
        playerAudio.PlayGroundPoundImpact();
    }

    private void GroundPoundLaunch()
    {
        Collider[] nearbyEnemies = Physics.OverlapCapsule(transform.position, transform.position, groundPoundLaunchRange);
        foreach(Collider enemy in nearbyEnemies)
        {
            if(enemy.CompareTag("Hurtbox"))
            {
                CharacterController enemyController = enemy.transform.parent.GetComponent<CharacterController>();
                if(enemyController)
                {
                    float launchStrength = Random.Range(groundPoundLaunchStrengthRange.x, groundPoundLaunchStrengthRange.y);
                    enemyController.Move(new Vector3(0, launchStrength, 0));
                }
                Health targetHealth = enemy.transform.parent.GetComponent<Health>();
                targetHealth.TakeDamage(groundPoundLaunchDamage);
            }

            if(enemy.GetComponent<Rigidbody>() != null)
            {
                float launchStrength = Random.Range(groundPoundLaunchStrengthRange.x, groundPoundLaunchStrengthRange.y);
                enemy.GetComponent<Rigidbody>().AddForce(new Vector3(0, launchStrength * rigirbodyMultipier, 0), ForceMode.Impulse);
            }
        }
    }

    public void Slide()
    {
        if(horizontalVelocity.magnitude < 0.1f || isDashing || isSliding || !canMove) return;
        playerCamera.position = new Vector3 (playerCamera.position.x, playerCamera.position.y - slidingCameraHeight, playerCamera.position.z);

        //slideParticle.Play();

        isSliding = true;
    }

    public void CancelSlide()
    {
        if(!isSliding) return;

        //slideParticle.Stop();

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
        playerAudio.PlayJumpSFX();

        yield return new WaitForSeconds(longJumpLength);
        isLongJumping = false;
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
        ceilingCheck.position = playerCamera.position;
    }

    public void CancelAllActions()
    {
        CancelSlide();
        isLongJumping = false;
        isDashing = false;
        isGroundPounding = false;
    }
}
