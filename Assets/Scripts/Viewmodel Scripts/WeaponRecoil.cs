using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    //Initilizes starting position of weapon and target position for recoil
    [HideInInspector]public Vector3 currentRotation, targetRotation, targetPosition, currentPosition, initialGunPosition;

    //create reference to sway & bob script so gun wil return to wherever the sway and bob script says it should
    [Header("References")]

    [Header("Recoil Values")]
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;

    [SerializeField] float kickBackZ;

    //How snappy should the recoil be
    public float snap;
    //How fast should it return to its original value
    public float returnAmount;

    private void Start()
    {
        initialGunPosition = transform.localPosition;
    }
    private void Update()
    {
        ReturnToCenter();
        BackToOriginalPosition();
    }

    //sets values needed for recoil
    public void ReturnToCenter()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snap);
    }
    
    //performs recoil plus kickback 
    public void Recoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(recoilZ, recoilZ));
    }

    //puts the gun back to its original position
    public void BackToOriginalPosition()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snap);
    }
}
