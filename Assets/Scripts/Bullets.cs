using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [HideInInspector] public float destroyTime = 5;

    //Script variables
    [HideInInspector] public int damage;

    //Required components
    private Health healthToDamage;

    private void Start() 
    {
        StartCoroutine("DestroyTimer");
        StartCoroutine("ActivateCollider");
    }

    private void OnCollisionEnter(Collision other) 
    {
        //Ignore player and other bullets
        if(other.gameObject.CompareTag("Hurtbox"))    
        {   
            healthToDamage = other.gameObject.transform.parent.gameObject.GetComponent<Health>();
            DealDamage();
        }
        else
        {
            if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bullet")) return;
            Destroy(gameObject);
        }   
    }

    private void OnTriggerEnter(Collider other) 
    {
        //Ignore player and other bullets
        if(other.gameObject.CompareTag("Hurtbox"))    
        {   
            healthToDamage = other.gameObject.transform.parent.gameObject.GetComponent<Health>();
            DealDamage();
        }
        else
        {
            if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bullet")) return;
            Destroy(gameObject);
        }   
    }

    private void DealDamage()
    {
        healthToDamage.TakeDamage(damage);
        Destroy(gameObject);
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }

    private IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(0.1f);

        GetComponent<Collider>().enabled = true;
    }
}
