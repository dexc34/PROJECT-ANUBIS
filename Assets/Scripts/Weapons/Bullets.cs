using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    [HideInInspector] public float destroyTime = 5;
    private GameObject bulletHole;
    private Vector3 holePosition;
    private Quaternion holeRotation;

    //Script variables
    [HideInInspector] public float damage;
    [HideInInspector] public float damageMultipler;
    [HideInInspector] public int penetrationAmmout;

    //Required components
    private Health healthToDamage;

    private void Start() 
    {
        StartCoroutine("DestroyTimer");
    }

    private void OnCollisionEnter(Collision other) 
    {
        Instantiate(bulletHole, holePosition, holeRotation).transform.parent = other.transform;
        if(other.gameObject.CompareTag("Hurtbox"))    
        {   
            healthToDamage = other.gameObject.transform.parent.gameObject.GetComponent<Health>();
            DealDamage(other.gameObject.name);
            Destroy(gameObject);
            //BulletPentrationCounter();
        }
        else
        {
            Destroy(gameObject);
        }   
    }

    private void DealDamage(string hurtboxHit)
    {
        //Deal crit damage to critical hurtboxes
        if(hurtboxHit.Contains("Critical Hurtbox")) damage *= damageMultipler;
            
        healthToDamage.TakeDamage(damage);
        
    }

    private void BulletPentrationCounter()
    {
        penetrationAmmout --;
        if(penetrationAmmout <=0 ) Destroy(gameObject);
        
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }

    public void GetBulletHoleInfo(GameObject holePrefab, Vector3 holePos, Quaternion holeRot)
    {
        bulletHole = holePrefab;
        holePosition = holePos;
        holeRotation = holeRot;
    }
}
