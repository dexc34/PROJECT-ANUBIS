using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    //Editor tools
    [SerializeField] private InputAction move;

    //Script variables
    private bool isShoothing = false;

    //Required components
    private Movement movementScript;
    private Gun gunScript;
    private SecondaryAbility secondaryAbilityScript;

    private void Awake() 
    {
        movementScript = GetComponent<Movement>();
        move.Enable();

        gunScript = GetComponent<Gun>();

        secondaryAbilityScript = GetComponent<SecondaryAbility>();
    }

    private void Update() 
    {
        movementScript.horizontalVelocity = move.ReadValue<Vector2>();

        if(!isShoothing) return;
        gunScript.FireBullet();
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

    public void GroundPound(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        movementScript.GroundPound();
    }

    //Gun actions
    public void Shoot(InputAction.CallbackContext context)
    {
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
        StartCoroutine(gunScript.Reload());
    }

    //Secondary ability actions
    public void ActivateAbility(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        secondaryAbilityScript.UseAbility();
    }
}
