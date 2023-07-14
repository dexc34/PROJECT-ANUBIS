using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathOfRa : MonoBehaviour
{
    //Editor Tools
    [SerializeField]
    private bool stopAllFunctionality = false;

    [Header("Stats")]
    public float cooldown;

    [SerializeField]
    [Tooltip("How much each hit of the laser will deal")]
    private int damagePerShot;

    [SerializeField]
    [Tooltip("How many hits will occur every second while aiming at something")]
    private int shotsPerSecond;

    [SerializeField]
    [Tooltip("How many units the raycast will go for when checking for targets")]
    private float range;

    [SerializeField]
    [Tooltip("What layers does the raycast check for")]
    private LayerMask layersToCheck;

    [SerializeField]
    [Tooltip("How much force the beam will apply to rigidbodies")]
    private float beamForce;

    [SerializeField]
    [Tooltip("How long in seconds will te beam last for")]
    private float beamDuration;

    [SerializeField]
    private GameObject beamPrefab;

    //Script variables
    private bool canDamage = true;
    private bool isFiring;
    private float raycastVisualRange;

    //Required components
    private Transform virtualCamera;
    private Transform particleTransform;
    private LineRenderer beamVisuals;
    private GameObject beamContainer;
    private Ray beamRay;
    private Gun gunScript;

    private void Start()
    {
        if (stopAllFunctionality) return;

        GetStats(GameObject.Find("Secondary Ability Manager").GetComponent<WrathOfRa>());

        if (gameObject.CompareTag("Player")) virtualCamera = GetComponentInChildren<CameraMove>().transform;
        else virtualCamera = transform;

        beamContainer = Instantiate(beamPrefab, virtualCamera.position, virtualCamera.rotation);
        beamContainer.transform.parent = transform;
        beamVisuals = beamContainer.GetComponent<LineRenderer>();
        particleTransform = beamContainer.GetComponentInChildren<ParticleSystem>().transform;
        gunScript = GetComponent<Gun>();
        beamContainer.SetActive(false);

        //Set layer mask
        if (transform.CompareTag("Player")) layersToCheck &= ~(1 << 7);
        else layersToCheck &= ~(1 << 10);
    }

    private void Update()
    {
        if (stopAllFunctionality) return;
        if (!isFiring) return;

        beamVisuals.SetPosition(0, transform.position);
        beamRay = new Ray(virtualCamera.position, virtualCamera.forward);

        if (Physics.Raycast(beamRay, out RaycastHit hitInfo, range, layersToCheck))
        {
            beamVisuals.SetPosition(1, beamRay.GetPoint(hitInfo.distance));
            particleTransform.position = beamRay.GetPoint(hitInfo.distance);
            if (hitInfo.transform.CompareTag("Hurtbox"))
            {
                if (canDamage)
                {
                    hitInfo.transform.parent.gameObject.GetComponent<Health>().TakeDamage(damagePerShot);
                    StartCoroutine("DamageCooldown");
                }
            }

            Rigidbody rb = hitInfo.transform.gameObject.GetComponent<Rigidbody>();
            if (rb)
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

    public void UseWrathOfRa()
    {
        StopCoroutine("StopBeam");
        beamContainer.SetActive(true);
        gunScript.interruptFire = true;
        isFiring = true;
        StartCoroutine("StopBeam");
    }

    private IEnumerator StopBeam()
    {
        yield return new WaitForSeconds(beamDuration);
        isFiring = false;
        beamContainer.SetActive(false);
        gunScript.interruptFire = false;
    }

    private IEnumerator DamageCooldown()
    {
        if (!canDamage) yield break;
        canDamage = false;

        yield return new WaitForSeconds(1 / shotsPerSecond);

        canDamage = true;
    }

    private void GetStats(WrathOfRa wrathOfRaScriptToPullFrom)
    {
        cooldown = wrathOfRaScriptToPullFrom.cooldown;
        damagePerShot = wrathOfRaScriptToPullFrom.damagePerShot;
        shotsPerSecond = wrathOfRaScriptToPullFrom.shotsPerSecond;
        range = wrathOfRaScriptToPullFrom.range;
        layersToCheck = wrathOfRaScriptToPullFrom.layersToCheck;
        beamForce = wrathOfRaScriptToPullFrom.beamForce;
        beamDuration = wrathOfRaScriptToPullFrom.beamDuration;
        beamPrefab = wrathOfRaScriptToPullFrom.beamPrefab;
    }

    private void OnDestroy()
    {
        Destroy(beamContainer);
    }
}
