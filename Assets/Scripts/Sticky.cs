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
        rb.velocity = new Vector3(0,0,0);
        rb.isKinematic = true;
        transform.parent = other.transform;
        hasStuck = true;
    }
}
