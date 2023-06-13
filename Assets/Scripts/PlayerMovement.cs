using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private InputAction move;
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private int amountOfJumps;
    [SerializeField] private float dashLenght;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float gravity;
    [SerializeField] private float gravityScale;
    private float yMovement;
    private Vector3 moveDirection;



    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
        move.Enable();
    }

    private void Update() 
    {
        ApplyGravity();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        //No gravity gets applied if grounded
        if (characterController.isGrounded && yMovement < 0)
        {
            yMovement = -1;
        }
        //Apply gravity when not grounded
        else
        {
            yMovement += gravity * gravityScale * Time.deltaTime;
        }
        moveDirection.y = yMovement;
    }

    private void ApplyMovement()
    {
        Vector2 inputReadings = move.ReadValue<Vector2>();
        moveDirection = transform.right * inputReadings.x + transform.forward * inputReadings.y;
        moveDirection = new Vector3(moveDirection.x, yMovement, moveDirection.z);
        //ApplyGravity();
        characterController.Move(moveDirection * speed * Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!characterController.isGrounded) return;
        if(context.performed)
        {
            yMovement += jumpHeight;
        }
    }


}
