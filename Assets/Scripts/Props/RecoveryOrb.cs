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
    [HideInInspector] public bool recoverAmmo;

    [HideInInspector] public float healthToRecover;

    [HideInInspector] public int staminaToRecover;

    [HideInInspector] public float ammoToRecoverPercent;

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
        if (other.name == "Player")
        {
            GameObject target = other.gameObject;

            if (recoverHealth) RecoverHealth(target);

            if (recoverStamina) RecoverStamina(target);

            if (recoverAbility) RecoverAbility(target);

            if (recoverAmmo) RecoverAmmo(target);

            if (recovered)
            {
                audioSource.Play();
                meshRenderer.enabled = false;
                trigger.enabled = false;
                if (canRespawn) StartCoroutine("OrbCooldown");
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

    private void RecoverHealth(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        if (health.currentHealth < health.maxHealth)
        {
            target.GetComponent<Health>().Heal(healthToRecover);
            recovered = true;
        }
        else Debug.Log("Already at max health");

    }

    private void RecoverStamina(GameObject target)
    {
        Movement movement = target.GetComponent<Movement>();
        if (movement.currentStamina < movement.maxStamina)
        {
            movement.RecoverStamina(staminaToRecover);
            recovered = true;
        }
        else Debug.Log("Already has max stamina");
    }

    private void RecoverAbility(GameObject target)
    {
        if (!target.GetComponent<SecondaryAbility>().canUseAbility)
        {
            target.GetComponent<SecondaryAbility>().ResetCooldown();
            recovered = true;
        }
        else Debug.Log("Already has ability");
    }

    private void RecoverAmmo(GameObject target)
    {
        Gun gun = target.GetComponent<Gun>();
        if (gun.currentAmmo < gun.totalAmmo)
        {
            float maxAmmo = gun.totalAmmo;
            float ammoToRecover = (maxAmmo / 100) * ammoToRecoverPercent;
            Debug.Log(ammoToRecover);
            gun.RecoverAmmo(((int)ammoToRecover));
            recovered = true;
        }
        else Debug.Log("Already at max ammo");
    }
}
