using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leap : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    private bool stopAllFunctionality = false;


    [Header ("Leap settings")]

    [Tooltip ("How long it takes for the ability to become available after being used")]
    public float cooldown;
    
    [SerializeField]
    [Tooltip ("How much horizontal speed the player will gain during the leap")]
    private float leapHorizontalSpeed;

    [SerializeField]
    [Tooltip ("How much vertical force will be applied to the player at the start of the leap")]
    private float leapHeight;


    [Header ("Pounce settings")]

    [SerializeField]
    [Tooltip ("How much damage targets caught inside pounce range will take")]
    private float pounceDamage;

    [SerializeField]
    [Tooltip ("How big of a radius the pounce will have")]
    private float pounceRange;

    [SerializeField]
    [Tooltip ("How much force is applied to character controllers caught in the pounce")]
    private float pounceStength;

    [SerializeField]
    [Tooltip ("How much the pounce strength will be multiplied by for rigidbodies")]
    private float rigirbodyMultipier;

    [SerializeField]
    [Tooltip ("How high the pounce bounce will be")]
    private float bounceHeight;


    [Header ("Audio")]
    
    [SerializeField]
    private AudioClip leapSFX;

    [SerializeField]
    private AudioClip pounceSFX;

    //Script variables
    private int layerToIgnore;

    //Required components
    private Movement movementScript;
    private CharacterController characterController;
    private AudioSource audioSource;
    
    private void Start() 
    {
        if(stopAllFunctionality) return;

        GetStats(GameObject.Find("Secondary Ability Manager").GetComponent<Leap>());
        movementScript = GetComponent<Movement>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() 
    {
        if(stopAllFunctionality) return;
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
        characterController.Move(new Vector3(movementScript.moveDirection.x * leapHorizontalSpeed, movementScript.moveDirection.y * movementScript.speed, movementScript.moveDirection.z * leapHorizontalSpeed) * Time.deltaTime);
    }

    private void Pounce()
    {
        audioSource.PlayOneShot(pounceSFX);

        Collider[] nearbyEnemies = Physics.OverlapCapsule(transform.position, transform.position, pounceRange);
        foreach(Collider enemy in nearbyEnemies)
        {
            if(enemy.CompareTag("Hurtbox") && enemy.gameObject.layer != gameObject.layer)
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
        movementScript.yVelocity = bounceHeight;
    }


    public void UseLeap()
    {
        //If player isn't moving, cancel leap, and refill ability cooldown
        if(new Vector2 (movementScript.moveDirection.x, movementScript.moveDirection.z).magnitude < 0.1f)
        {    
            GetComponent<SecondaryAbility>().ResetCooldown();
            return;
        }

        //Cancel all movement
        movementScript.CancelAllActions();
        movementScript.canMove = false;
        movementScript.yVelocity = leapHeight;

        audioSource.PlayOneShot(leapSFX);

        //Add a jump if the player doesn't have any left
        if(movementScript.currentJumps <= 0) movementScript.currentJumps ++;
        
        movementScript.isLeaping = true;
    }

    private void GetStats(Leap leapScriptToPullFrom)
    {
        //Leap stats
        cooldown = leapScriptToPullFrom.cooldown;
        leapHorizontalSpeed = leapScriptToPullFrom.leapHorizontalSpeed;
        leapHeight = leapScriptToPullFrom.leapHeight;

        //Pounce stats
        pounceDamage = leapScriptToPullFrom.pounceDamage;
        pounceRange = leapScriptToPullFrom.pounceRange;
        pounceStength = leapScriptToPullFrom.pounceStength;
        rigirbodyMultipier = leapScriptToPullFrom.rigirbodyMultipier;
        bounceHeight = leapScriptToPullFrom.bounceHeight;

        //Audio
        leapSFX = leapScriptToPullFrom.leapSFX;
        pounceSFX = leapScriptToPullFrom.pounceSFX;
    }
}
