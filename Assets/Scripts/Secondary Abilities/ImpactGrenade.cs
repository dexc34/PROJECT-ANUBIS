using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGrenade : MonoBehaviour
{
    [HideInInspector] public float cooldown = 8;
    private GameObject grenadePrefab;
    //
    private Transform originPoint;

    private void Start() 
    {
        grenadePrefab = (GameObject) Resources.Load("Grenade Alt");
    }
    public void UseImpactGrenade(Transform origin)
    {
        originPoint = origin;
        GameObject grenade = Instantiate(grenadePrefab, originPoint.position + originPoint.forward, originPoint.rotation);
        Explosion explosionScript = grenade.GetComponent<Explosion>();
        grenade.GetComponent<Rigidbody>().AddForce(originPoint.forward * explosionScript.grenadeThrowForce, ForceMode.Impulse);
    }
}
