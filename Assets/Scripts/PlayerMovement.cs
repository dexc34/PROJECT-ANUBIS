using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private InputAction move;
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private int amountOfJumps = 2;
    private int currentJumps;
    [SerializeField] private float dashLenght;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float gravity;
    [SerializeField] private float gravityScale;
    private float yMovement;
    private Vector3 moveDirection;

    private AudioSource funnyBoom;



    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
        funnyBoom = GetComponent<AudioSource>();
        move.Enable();
    }

    private void Update() 
    {
        if(transform.position.y < -7)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if(Input.GetMouseButtonDown(0))
        {
            funnyBoom.Play();
        }

        ApplyGravity();
        ApplyMovement();

        if(characterController.isGrounded && currentJumps != amountOfJumps)
        {
            currentJumps = amountOfJumps;
        }
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
        if(!characterController.isGrounded && currentJumps == 0) return;
        if(context.performed)
        {
            yMovement = jumpHeight;
            currentJumps --;
            Debug.Log(currentJumps);
        }
    }


}
