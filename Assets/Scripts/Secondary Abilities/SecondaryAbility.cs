using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class SecondaryAbility : MonoBehaviour
{
    [SerializeField] private bool overrideCooldown = false;
    [HideInInspector] public float abilityCooldown;

    //Script variables
    [HideInInspector] public bool canUseAbility = true;
    private UnityEvent secondaryFunction = new UnityEvent();

    //Removes any extra secondary ability scripts

    public void UpdateSecondary(SecondaryDropdownOptions secondaryEnum)
    {
        RemoveUnnecessaryComponents();
        secondaryFunction.RemoveAllListeners();

        //Adds necessary scripts for the required secondary ability, and adds event listener
        try
        {
            switch(secondaryEnum)
            {
                case SecondaryDropdownOptions.ImpactGrenade:
                    ImpactGrenade tempGrenade = gameObject.AddComponent(typeof(ImpactGrenade)) as ImpactGrenade;
                    abilityCooldown = tempGrenade.cooldown;
                    secondaryFunction.AddListener(tempGrenade.UseImpactGrenade);
                    break;

                case SecondaryDropdownOptions.Barrage:
                    Barrage tempBarrage = gameObject.AddComponent(typeof (Barrage)) as Barrage;
                    abilityCooldown = tempBarrage.cooldown;
                    secondaryFunction.AddListener(tempBarrage.UseBarrage);
                    break;

                case SecondaryDropdownOptions.WrathOfRa:
                    WrathOfRa tempWrathOfRa = gameObject.AddComponent(typeof (WrathOfRa)) as WrathOfRa;
                    abilityCooldown = tempWrathOfRa.cooldown;
                    secondaryFunction.AddListener(tempWrathOfRa.UseWrathOfRa);
                    break;

                case SecondaryDropdownOptions.CloserToThePrey:
                    Debug.Log("No closer to the prey script has been made yet");
                    break;    
            }
        }
        catch
        {
            Debug.Log("No secondary detected");
        }

        if(overrideCooldown) abilityCooldown = 0;
    }
    private void RemoveUnnecessaryComponents()
    {
        //Destroy impact grenade
        ImpactGrenade tempImpactGrenade = GetComponent<ImpactGrenade>();
        if(tempImpactGrenade) Destroy(tempImpactGrenade);

        Barrage tempBarrage = GetComponent<Barrage>();
        if(tempBarrage) Destroy(tempBarrage);

        WrathOfRa tempWrathOfRa = GetComponent<WrathOfRa>();
        if(tempWrathOfRa) Destroy(tempWrathOfRa);
        //Add new scripts to destroy here
    }

    public void UseAbility()
    {
        if(!canUseAbility) return;
        canUseAbility = false;
        secondaryFunction.Invoke();
        StartCoroutine("AbilityCooldown");
    }

    private IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(abilityCooldown);

        canUseAbility = true;
    }
}
