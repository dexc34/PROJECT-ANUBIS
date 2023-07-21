using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    //Editor tools
    [SerializeField] private InputAction move;
    [SerializeField] 
    [Tooltip ("How long crouch button stays registered after letting go")]
    private float crouchCancelDelay;

    //Script variables
    private bool isShoothing = false;
    private bool isCrouching = false;

    //Required components
    private Movement movementScript;
    private Gun gunScript;
    private SecondaryAbility secondaryAbilityScript;
    private PlayerHackingScript hackingScript;
    private Melee meleeScript;
    private CharacterController characterController;

    private void Awake() 
    {
        movementScript = GetComponent<Movement>();
        move.Enable();

        gunScript = GetComponent<Gun>();

        secondaryAbilityScript = GetComponent<SecondaryAbility>();

        hackingScript = GetComponent<PlayerHackingScript>();

        meleeScript = GetComponent<Melee>();

        characterController = GetComponent<CharacterController>();
    }

    private void Update() 
    {
        //Update movement direction
        movementScript.horizontalVelocity = move.ReadValue<Vector2>();

        //Used for automatic weapons
        if(isShoothing) gunScript.FireBullet();

        //Sliding vs bouncing on touching ground while ground pounding
        
        if(characterController.isGrounded)
        {
            if(movementScript.isGroundPounding)
            {
                if(isCrouching)
                {
                    movementScript.isGroundPounding = false;
                    movementScript.Slide();
                }
                else
                {
                    movementScript.isGroundPounding = false;
                    movementScript.GroundPoundBounce();
                }
            }
            
        }
    }

    //Movement functions
    public void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        movementScript.Jump();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        StartCoroutine(movementScript.Dash());
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            isCrouching = false; 
            movementScript.CancelSlide();
            return;
        }

        if(context.performed)
        {
            isCrouching = true;
            //StartCoroutine(CrouchCancel(context));
        }

        if(characterController.isGrounded)
        {
            movementScript.Slide();
        }
        else
        {
            movementScript.GroundPound();   
        }
    }

    private IEnumerator CrouchCancel(InputAction.CallbackContext context)
    {
        yield return new WaitForSeconds(crouchCancelDelay);
        if(context.performed) isCrouching = true;
        else if(context.canceled) isCrouching = false;
    }

    //Gun actions
    public void Shoot(InputAction.CallbackContext context)
    {
        //If primary is melee ignore gun script and instead perform a melee attack
        if(gunScript.primaryIsMelee)
        {
            meleeScript.UseMelee();
            return;
        }

        if(gunScript.isAutomatic)
        {
            if(context.performed)
            {
                isShoothing = true;
            }

            if(context.canceled)
            {
                isShoothing = false;
            }
        }

        //If weapon isn't automatic only register input when button is first pressed
        if(!gunScript.isAutomatic && context.performed)
        {
            gunScript.FireBullet();
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        gunScript.Reload();
    }

    //Secondary ability actions
    public void ActivateAbility(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        secondaryAbilityScript.UseAbility();
    }

    //Hacking
    public void Hack(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            hackingScript.hackInputDetected = false;
            hackingScript.needSecondInput = false;
        }

        if(hackingScript.needSecondInput) return;
        
        if(context.performed)
        {
            hackingScript.hackInputDetected = true;
        }
    }

    public void Melee(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        meleeScript.UseMelee();
    }
}
