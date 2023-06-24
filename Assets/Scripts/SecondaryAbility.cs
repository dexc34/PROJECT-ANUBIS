using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SecondaryAbility : MonoBehaviour
{
    //Editor tools
    [Header ("General ability settings")]

    [SerializeField]
    [Tooltip ("Time in seconds until ability is available again after using it")]
    private float abilityCooldown;


    [Header ("Grenade settings found inside the prefab")]

    [SerializeField]
    [Tooltip ("Grenade prefab goes here (damage, timing, and force applied to targets are changed inside the prefab)")]
    private GameObject grenadePrefab;

    //Script variables
    private string secondaryFunctionToCall;
    private bool canUseAbility = true;

    //Required components
    private Transform virtualCamera;   

    //Changes what the secondary ability based on what gun you're using (called from the gun script when updating stats)
    public void UpdateSecondary(string secondaryAbilityName, Transform newCamera)
    {
        secondaryFunctionToCall = secondaryAbilityName;
        virtualCamera = newCamera;
    }

    public void ActivateAbility(InputAction.CallbackContext context)
    {
        if(!context.performed || !canUseAbility) return;
        canUseAbility = false;
        Invoke(secondaryFunctionToCall, 0);
        StartCoroutine("AbilityCooldown");
    }

    private IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(abilityCooldown);

        canUseAbility = true;
        Debug.Log("Ability available");
    }

    //---------------------------Ability Specific Functions-------------------------------------------------------------------------------------------
    private void ImpactGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, virtualCamera.position + virtualCamera.forward, virtualCamera.rotation);
        Explosion explosionScript = grenade.GetComponent<Explosion>();
        grenade.GetComponent<Rigidbody>().AddForce(virtualCamera.forward * explosionScript.grenadeThrowForce, ForceMode.Impulse);
    }

    private void Barrage()
    {
        Debug.Log("Do barrage");
    }

    private void WrathOfRa()
    {
        Debug.Log("Wrath the rath");
    }

    private void CloserToThePrey()
    {
        Debug.Log("Pray for the prey");
    }
}
