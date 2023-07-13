using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leap : MonoBehaviour
{
    //Editor tools
    public float cooldown = 6;

    [SerializeField]
    private float leapHorizontalSpeed = 18;

    [SerializeField]
    private float leapHeight = 2.5f;

    [SerializeField]
    private float pounceDamage = 50;

    [SerializeField]
    private float pounceRange = 10;

    [SerializeField]
    private float pounceStength = 3;

    [SerializeField]
    private float rigirbodyMultipier = 100;

    //Required components
    private Movement movementScript;
    private CharacterController characterController;
    
    private void Start() 
    {
        movementScript = GetComponent<Movement>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update() 
    {
        if(!movementScript) return;
        if(!movementScript.isLeaping)
        {
            if(!movementScript.canMove)
            {
                movementScript.canMove = true;
                movementScript.canAct = true;
            }
            return;
        }

        ApplyMovement();

        if(characterController.isGrounded) Pounce();
    }

    private void ApplyMovement()
    {
        movementScript.ApplyGravity();
        if(new Vector2 (movementScript.moveDirection.x, movementScript.moveDirection.z).magnitude < 0.1f)
        {
            return;
        }
        characterController.Move(new Vector3(movementScript.moveDirection.x * leapHorizontalSpeed, movementScript.moveDirection.y * movementScript.speed, movementScript.moveDirection.z * leapHorizontalSpeed) * Time.deltaTime);
    }

    private void Pounce()
    {
        Collider[] nearbyEnemies = Physics.OverlapCapsule(transform.position, transform.position, pounceRange);
        foreach(Collider enemy in nearbyEnemies)
        {
            if(enemy.CompareTag("Hurtbox"))
            {
                CharacterController enemyController = enemy.transform.parent.GetComponent<CharacterController>();
                if(enemyController)
                {
                    enemyController.Move(new Vector3(0, pounceStength, 0));
                }
                Health targetHealth = enemy.transform.parent.GetComponent<Health>();
                targetHealth.TakeDamage(pounceDamage);
            }

            if(enemy.GetComponent<Rigidbody>() != null)
            {
                enemy.GetComponent<Rigidbody>().AddForce(new Vector3(0, pounceStength * rigirbodyMultipier, 0), ForceMode.Impulse);
            }
        }

        movementScript.isLeaping = false;
        movementScript.canMove = true;
    }


    public void UseLeap()
    {
        //Cancel all movement
        movementScript.CancelAllActions();
        movementScript.canMove = false;
        movementScript.yVelocity = leapHeight;
        
        movementScript.isLeaping = true;
    }
}
