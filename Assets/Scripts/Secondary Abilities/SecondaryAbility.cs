using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public enum SecondaryDropdownOptions{ImpactGrenade, Barrage, WrathOfRa, CloserToThePrey, Leap};
public class SecondaryAbility : MonoBehaviour
{
    [SerializeField]
    SecondaryDropdownOptions secondaryType = new SecondaryDropdownOptions();

    [SerializeField] private bool overrideCooldown = false;
    [HideInInspector] public float abilityCooldown;

    //Script variables
    [HideInInspector] public bool canUseAbility = true;

    //Required components
    private UnityEvent secondaryFunction = new UnityEvent();
    private Transform secondaryOrigin;
    private GameObject secondaryManager;

    private void Start() 
    {
        if(transform.CompareTag("Player")) secondaryOrigin = GetComponentInChildren<CameraMove>().transform;
        else secondaryOrigin = transform;

        secondaryManager = GameObject.Find("Secondary Ability Manager");

        UpdateSecondary(this);
    }

    public void UpdateSecondary(SecondaryAbility secondaryAbilityScriptToPullFrom)
    {
        RemoveUnnecessaryComponents();
        secondaryFunction.RemoveAllListeners();

        if(transform.CompareTag("Player")) secondaryOrigin = GetComponentInChildren<CameraMove>().transform;
        else secondaryOrigin = transform;

        secondaryType = secondaryAbilityScriptToPullFrom.secondaryType;

        //Adds necessary scripts for the required secondary ability, and adds event listener
        try
        {
            switch(secondaryType)
            {
                case SecondaryDropdownOptions.ImpactGrenade:
                    ImpactGrenade tempGrenade = gameObject.AddComponent<ImpactGrenade>();
                    abilityCooldown = secondaryManager.GetComponent<ImpactGrenade>().cooldown;
                    secondaryFunction.AddListener(delegate {tempGrenade.UseImpactGrenade(secondaryOrigin);});
                    break;

                case SecondaryDropdownOptions.Barrage:
                    Barrage tempBarrage = gameObject.AddComponent<Barrage>();
                    abilityCooldown = secondaryManager.GetComponent<Barrage>().cooldown;
                    secondaryFunction.AddListener(delegate {tempBarrage.UseBarrage(secondaryOrigin);});
                    break;

                case SecondaryDropdownOptions.WrathOfRa:
                    WrathOfRa tempWrathOfRa = gameObject.AddComponent<WrathOfRa>();
                    abilityCooldown = secondaryManager.GetComponent<WrathOfRa>().cooldown;
                    secondaryFunction.AddListener(tempWrathOfRa.UseWrathOfRa);
                    break;

                case SecondaryDropdownOptions.CloserToThePrey:
                    CloserToThePrey tempCloserToThePrey = gameObject.AddComponent<CloserToThePrey>();
                    abilityCooldown = secondaryManager.GetComponent<CloserToThePrey>().cooldown;
                    secondaryFunction.AddListener(delegate {tempCloserToThePrey.UseCloserToThePrey(secondaryOrigin);});
                    break;    

                case SecondaryDropdownOptions.Leap:
                    Leap tempLeap = gameObject.AddComponent<Leap>();
                    abilityCooldown = secondaryManager.GetComponent<Leap>().cooldown;
                    secondaryFunction.AddListener(tempLeap.UseLeap);
                    break;
            }
        }
        catch
        {
            Debug.Log("No secondary detected");
        }

        ResetCooldown();
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

        CloserToThePrey tempCloserToThePrey = GetComponent<CloserToThePrey>();
        if(tempCloserToThePrey) Destroy(tempCloserToThePrey);
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

    public void ResetCooldown()
    {
        StopCoroutine("AbilityCooldown");
        canUseAbility = true;
    }
}
