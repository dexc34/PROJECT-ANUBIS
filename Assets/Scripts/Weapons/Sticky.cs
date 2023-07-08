using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticky : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasStuck = false;
    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(hasStuck) return;
        Destroy(rb);

        Transform hurtbox = other.transform.Find("Hurtbox");
        if(hurtbox != null)
        {
            transform.parent = hurtbox;
        }
        else
        {
            transform.parent = other.transform;
        }
        
        hasStuck = true;
    }
}
