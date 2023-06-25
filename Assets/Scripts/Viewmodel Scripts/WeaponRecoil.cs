using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    //Initilizes starting position of weapon and target position for recoil
    Vector3 currentRotation, targetRotation, targetPosition, currentPosition, initialGunPosition;

    [Header("Recoil Values")]
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;

    [SerializeField] float kickBackZ;

    public float snap;
    public float returnAmount;

    private void Start()
    {
        initialGunPosition = transform.localPosition;
    }

    private void Update()
    {
        SetValues();
        Kickback();
    }
    void SetValues()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snap);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void Recoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(recoilZ, recoilZ));
    }

    void Kickback()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snap);
        transform.localPosition = currentPosition;
    }
}
