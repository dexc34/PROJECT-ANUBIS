using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClipPrevention : MonoBehaviour
{
    [Header("Clip Projection Value Checks")]
    public float checkDistance;
    public Vector3 newDirection;

    float lerpPos;
    RaycastHit hit;

    GameObject clipProjector;

    private void Start()
    {
        clipProjector = GameObject.Find("Clip Projector");
    }

    private void Update()
    {
        if (Physics.Raycast(clipProjector.transform.position, clipProjector.transform.forward, out hit, checkDistance))
            lerpPos = 1 - (hit.distance / checkDistance); //get a percentage from 0 to max distance 
        else
            lerpPos = 0; //if raycast hits nothing, set to 0

        Mathf.Clamp01(lerpPos);

        //"(Quaternion.Euler(Vector3.zero)" points straight ahead    |    "Quaternion.Euler(newDirection)" points away    |    "lerpPos" is the position between the two
        transform.localRotation = Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(newDirection), lerpPos); 


    }
}
