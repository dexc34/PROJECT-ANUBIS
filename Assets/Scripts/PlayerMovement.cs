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

    [Header ("Dash Settings")]
    [SerializeField] public int amountOfDashes = 2;
    [SerializeField] [Tooltip ("How fast does the player become while dashing")]private float dashSpeed;
    [SerializeField] [Tooltip ("How long does the dash go for in seconds")] public float dashDuration;
    [SerializeField] [Tooltip("How long does the player wait to use the dash again after it's done in seconds")] public float dashCooldown;

    [Header ("Gravity Settings")]
    [SerializeField] [Tooltip ("-1 to go down, 0 for no gravity, 1 to go up")] [Range (-1, 1)] private int gravity;
    [SerializeField] [Tooltip ("How strong is the gravity")] private float gravityScale;

    //Private script variables
    private float yVelocity; //Tracks vertical speed
    private Vector3 moveDirection; //Makes sure direction is always camera dependent 
    private int currentJumps; // Amount of jumps available to the player at any given time
    public int currentDashes; // Amount of dashes available to the player at any given time
    public bool dashCooldownDone = true;
    public bool isDashing = false;

    //Required components
    private CharacterController characterController; 
    private Transform ceilingCheck;

    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
        ceilingCheck = transform.Find("Ceiling Check");
        ChangeStats();
        currentDashes = amountOfDashes;
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
        }

        ApplyGravity();
        ApplyMovement();

        //Restores jumps on landing
        if(characterController.isGrounded && currentJumps != amountOfJumps)
        {
            currentJumps = amountOfJumps;
        }

        //Removes one jump when leaving the ground
        if(!characterController.isGrounded && currentJumps == amountOfJumps)
        {
            currentJumps --;
        }
    }

    private void ApplyGravity()
    {
        if(isDashing) return;
        //No gravity gets applied if grounded
        if (characterController.isGrounded && yVelocity < 0)
        {
            yVelocity = -0.1f;
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
        characterController.Move(moveDirection * speed * Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!characterController.isGrounded && currentJumps == 0) return;
        if(context.performed)
        {
            yVelocity = jumpHeight;
            currentJumps --;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(currentDashes == 0 || !context.performed || isDashing) return;
        StartCoroutine("Dashing");
    }

    public IEnumerator Dashing()
    {
        currentDashes --;
        yVelocity = 0;
        moveDirection.y = yVelocity;
        isDashing = true;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        //Dont start cooldown timer until the previous one is done
        while(!dashCooldownDone) yield return null;
    
        //Return ability to dash after cooldown
        dashCooldownDone = false;

        yield return new WaitForSeconds(dashCooldown);
        dashCooldownDone = true;
        currentDashes ++;
    }

    public void ChangeStats()
    {
        ceilingCheck.position = transform.Find("Player Camera").position;
    }
}
