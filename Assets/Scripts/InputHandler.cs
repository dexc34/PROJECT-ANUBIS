using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    //Required components
    private Movement movementScript;

    private void Start() 
    {
        movementScript = GetComponent<Movement>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        movementScript.Jump();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        movementScript.Dash();
    }

    public void GroundPound(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        movementScript.GroundPound();
    }
}
