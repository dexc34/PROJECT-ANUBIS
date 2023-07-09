using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class JumpPad : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("How much force will be applied to character controllers on contact with the jump pad")]
    private float padStrength;

    [SerializeField]
    private float rigidbodyForce;

    [SerializeField]
    [Tooltip ("Audio source that will be used to play the jump pad sound effect")]
    private AudioSource audioSource;

    [SerializeField]
    [Tooltip ("Clip variations that will be used for the jump pad")]
    private AudioClip[] soundEffects;

    //Script variables
    private Vector3 triggerSize;

    private void Start() 
    {
        triggerSize = transform.localScale;
    }

    private void Update() 
    {
        Collider[] objectsInTrigger = Physics.OverlapBox(transform.position, triggerSize, Quaternion.identity);
        foreach(Collider nearbyObjects in objectsInTrigger)
        {
            if(nearbyObjects.gameObject.CompareTag("Hurtbox"))
            {
                Movement movementScript = nearbyObjects.transform.parent.gameObject.GetComponent<Movement>();
                if(movementScript)
                {
                    movementScript.isGroundPounding = false;
                    if(movementScript.currentJumps <= 0) movementScript.currentJumps ++;
                    audioSource.PlayOneShot(soundEffects[Random.Range(0, soundEffects.Length)], 1);
                    movementScript.yVelocity = padStrength;
                }
            }

            Rigidbody rb = nearbyObjects.GetComponent<Rigidbody>();
            if(rb)
            {
                audioSource.PlayOneShot(soundEffects[Random.Range(0, soundEffects.Length)], 1);
                rb.AddForce(Vector3.up * rigidbodyForce, ForceMode.Impulse);
            }
        }
    }
}
