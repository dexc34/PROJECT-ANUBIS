using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryOrb : MonoBehaviour
{
    //Editor tools
    [HideInInspector] public bool canRespawn = true;
    [HideInInspector] public float orbCooldown;

    [HideInInspector] public bool recoverHealth;
    [HideInInspector] public bool recoverStamina;
    [HideInInspector] public bool recoverAbility;

    [HideInInspector] public float healthToRecover;

    [HideInInspector] public int staminaToRecover;

    //Script variables
    private bool recovered = false;

    //Required components
    private MeshRenderer meshRenderer;
    private Collider trigger;
    private AudioSource audioSource;

    private void Start() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        trigger = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other) 
    {
        if(other.name == "Player")    
        {
            if(recoverHealth)
            {
                Health health = other.GetComponent<Health>();
                if(health.currentHealth < health.maxHealth)
                {
                    other.GetComponent<Health>().Heal(healthToRecover);
                    recovered = true;
                } 
                else Debug.Log("ALready at max health");
            }

            if(recoverStamina)
            {
                Movement movement = other.GetComponent<Movement>();
                if(movement.currentStamina < movement.maxStamina)
                {
                    movement.RecoverStamina(staminaToRecover);
                    recovered = true;
                }
                else Debug.Log("Already max stamina");
            }

            if(recoverAbility)
            {
                if(!other.GetComponent<SecondaryAbility>().canUseAbility)
                {
                    other.GetComponent<SecondaryAbility>().ResetCooldown();
                    recovered = true;
                }
                else Debug.Log("Already has ability");
            }

            if(recovered)
            {
                audioSource.Play();
                meshRenderer.enabled = false;
                trigger.enabled = false;
                if(canRespawn) StartCoroutine("OrbCooldown");
            }
        }
    }

    private IEnumerator OrbCooldown()
    {
        yield return new WaitForSeconds(orbCooldown);

        meshRenderer.enabled = true;
        trigger.enabled = true;
        recovered = false;
    }
}
