using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5;

    //Script variables
    [HideInInspector] public int damage;

    //Required components
    private Health healthToDamage;

    private void Start() 
    {
        StartCoroutine("DestroyTimer");
    }

    private void OnCollisionEnter(Collision other) 
    {
        //Ignore player and other bullets
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bullet")) return;
        else if(other.gameObject.CompareTag("Hackable") || other.gameObject.CompareTag("Destructible"))    
        {   
            healthToDamage = other.gameObject.GetComponent<Health>();
            DealDamage();
        }
        else
        {
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
}
