using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathOfRa : MonoBehaviour
{
    //Script variables
    [HideInInspector] public float cooldown = 9;
    private int damagePerShot = 5;
    private int shotsPerSecond = 50;
    private bool canDamage = true;
    private float range = 50;
    private float beamForce = .2f;
    private float beamDuration = 6;
    private bool isFiring;
    private float raycastVisualRange;

    //Required components
    private Transform originPoint;
    private Transform particleTransform;
    private LineRenderer beamVisuals;
    private GameObject beamPrefab;
    private GameObject beamContainer;
    private Ray beamRay;
    private Gun gunScript;

    private void Start() 
    {    
        beamPrefab = (GameObject) Resources.Load("Beam");
        beamContainer = Instantiate(beamPrefab, originPoint.position, originPoint.rotation);
        beamContainer.transform.parent = transform;
        beamVisuals = beamContainer.GetComponent<LineRenderer>();
        particleTransform = beamContainer.GetComponentInChildren<ParticleSystem>().transform;
        gunScript = GetComponent<Gun>();
        beamContainer.SetActive(false);
    }

    private void Update() 
    {
        if(!isFiring) return;

        if(gunScript.canFire) gunScript.canFire = false;


        beamVisuals.SetPosition(0, transform.position);
        beamRay = new Ray(originPoint.position, originPoint.forward);

        if(Physics.Raycast(beamRay, out RaycastHit hitInfo, range, 15))
        {
            beamVisuals.SetPosition(1, beamRay.GetPoint(hitInfo.distance));
            particleTransform.position = beamRay.GetPoint(hitInfo.distance);
            if(hitInfo.transform.CompareTag("Hurtbox"))
            {
                if(canDamage)
                {
                    hitInfo.transform.parent.gameObject.GetComponent<Health>().TakeDamage(damagePerShot);
                    StartCoroutine("DamageCooldown");
                }
            }
            
            Rigidbody rb = hitInfo.transform.gameObject.GetComponent<Rigidbody>();
            if(rb)
            {
                Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                rb.AddForce(forceDirection * beamForce, ForceMode.Impulse);
            }
        }
        else
        {
            beamVisuals.SetPosition(1, beamRay.GetPoint(range));
            particleTransform.position = beamRay.GetPoint(range);
        }
    }

    public void UseWrathOfRa(Transform origin)
    {
        originPoint = origin;
        beamContainer.SetActive(true);
        isFiring = true;
        StartCoroutine("StopBeam");
    }

    private IEnumerator StopBeam()
    {
        yield return new WaitForSeconds(beamDuration);
        isFiring = false;
        beamContainer.SetActive(false);
        gunScript.canFire = true;
    }

    private IEnumerator DamageCooldown()
    {
        if(!canDamage) yield break;
        canDamage = false;

        yield return new WaitForSeconds(1/shotsPerSecond);

        canDamage = true;
    }

    private void OnDestroy() 
    {
        Destroy(beamContainer);
    }
}
