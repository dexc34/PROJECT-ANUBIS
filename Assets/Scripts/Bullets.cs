using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5;

    private void Start() 
    {
        StartCoroutine("DestroyTimer");
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("Hackable"))    
        {   
            DealDamage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DealDamage()
    {
        Debug.Log("Hit enemy");
        Destroy(gameObject);
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }
}
