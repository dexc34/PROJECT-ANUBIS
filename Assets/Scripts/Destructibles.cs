using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructibles : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("Function to call when destroyed")]
    private string funtionToCall;
    
    //Required components
    private Explosion explosionScript;
    private AudioSource audioSource;

    private void Start() 
    {
        if(GetComponent<Explosion>() != null) explosionScript = GetComponent<Explosion>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void Destroyed()
    {
        Invoke(funtionToCall, 0);
    }

    private void BarrelExplode()
    {
        explosionScript.Explode();
        audioSource.Play();
    }
}
