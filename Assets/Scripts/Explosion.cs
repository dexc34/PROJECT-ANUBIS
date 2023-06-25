using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("How much damage should it deal to targets hit")]
    private int damage;

    [SerializeField]
    [Tooltip ("How big the explosion radius will be")]
    private float explosionRange;

    [SerializeField]
    [Tooltip ("How strong the expliosion force applied to objects within range will be")]
    private float explosionForce;

    [Tooltip ("How much force should be applied to the object when spawning (only for grenade, keep as 0 for other uses)")]
    public float grenadeThrowForce;

    [SerializeField]
    [Tooltip ("Should explosion occur as soon as it hits something?")]
    private bool explodeOnImpact = false;

    [SerializeField]
    [Tooltip ("Completely ignore the timer")]
    private bool overrideTimer = false;

    [SerializeField]
    [Tooltip ("Time in seconds it takes for the explosion to occur")]
    private float timer = 2;

    [SerializeField]
    [Tooltip ("Particle effect to play when explosion happens")]
    private GameObject explosionVisuals;

    //Script variables
    private bool hasExploded = false;

    //Required components
    private ParticleSystem particles;

    private void Start() 
    {
        particles = GetComponentInChildren<ParticleSystem>();
        if(overrideTimer) return;
        StartCoroutine("StartExplosionTimer");
    }

    //Easily visualize the explosion range in scene view
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,explosionRange);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(!explodeOnImpact || other.gameObject.CompareTag("Player")) return;
        Explode();
    }

    private IEnumerator StartExplosionTimer()
    {
        yield return new WaitForSeconds(timer);

        Explode();
    }

    public void Explode()
    {
        if(hasExploded) return;
        hasExploded = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            }

            Health healthToDamage = nearbyObject.GetComponent<Health>();
            if(healthToDamage != null)
            {
                healthToDamage.TakeDamage(damage);
            }
        }

        //Unparents particle system to safely be able to destroy the gameoject without affecting visuals
        particles.gameObject.transform.parent = null;
        particles.Play();

        Destroy(gameObject);
    }
}
