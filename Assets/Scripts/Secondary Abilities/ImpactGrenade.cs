using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGrenade : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    private bool stopAllFunctionality = false;


    [Header ("Stats")]

    [Tooltip ("How long it takes for the ability to become available after being used")]
    public float cooldown;

    [SerializeField]
    [Tooltip ("What grenade should be spawned")]
    private GameObject grenadePrefab;


    [Header ("Audio")]

    [SerializeField]
    private AudioClip throwSFX;

    //Required components
    private Transform originPoint;
    private AudioSource audioSource;

    private void Start() 
    {
        if(stopAllFunctionality) return;

        GetStats(GameObject.Find("Secondary Ability Manager").GetComponent<ImpactGrenade>());
        audioSource = GetComponent<AudioSource>();
    }
    public void UseImpactGrenade(Transform origin)
    {
        originPoint = origin;
        GameObject grenade = Instantiate(grenadePrefab, originPoint.position + originPoint.forward, originPoint.rotation);
        Explosion explosionScript = grenade.GetComponent<Explosion>();
        grenade.GetComponent<Rigidbody>().AddForce(originPoint.forward * explosionScript.grenadeThrowForce, ForceMode.Impulse);
        audioSource.PlayOneShot(throwSFX);
    }

    private void GetStats(ImpactGrenade impactGrenadeScriptToPullFrom)
    {
        cooldown = impactGrenadeScriptToPullFrom.cooldown;
        grenadePrefab = impactGrenadeScriptToPullFrom.grenadePrefab;
        throwSFX = impactGrenadeScriptToPullFrom.throwSFX;
    }
}
