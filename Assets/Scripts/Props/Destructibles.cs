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

    private void Start() 
    {
        if(GetComponent<Explosion>() != null) explosionScript = GetComponent<Explosion>();
    }

    public void Destroyed()
    {
        Invoke(funtionToCall, 0);
    }

    private void BarrelExplode()
    {
        explosionScript.Explode();
    }
}
