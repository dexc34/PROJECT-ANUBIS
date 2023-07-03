using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    //Editor tools
    [SerializeField] private InputAction move;

    //Required components
    private Movement movementScript;

    private void Awake() 
    {
        movementScript = GetComponent<Movement>();
        move.Enable();
    }

    private void Update() 
    {
        movementScript.horizontalVelocity = move.ReadValue<Vector2>();
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
