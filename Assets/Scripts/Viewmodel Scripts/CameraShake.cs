using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineImpulseSource screenShake;
    /*[SerializeField] Camera cam;
    //[SerializeField] GameObject player;

    [Header("Shake Range")]
    [Tooltip("Is this shake affected by range? If not, the shake will happen gloablly")]
    [SerializeField] bool affectedByRange;
    [SerializeField] float range;

    [Header("Shake Settings")]
    [SerializeField] float duration = 0.2f;

    //Easily visualize the explosion range in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }*/
    public void ScreenShake(Vector3 mag)
    {
        screenShake.GenerateImpulse(mag);
    }

}

