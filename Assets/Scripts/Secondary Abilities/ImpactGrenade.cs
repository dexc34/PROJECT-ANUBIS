using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImpactGrenade : MonoBehaviour
{
    private GameObject grenadePrefab;

    [HideInInspector] public Transform virtualCamera;

    private void Start() 
    {
        virtualCamera = GetComponentInChildren<CameraMove>().gameObject.transform;
        grenadePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Grenade.prefab", typeof(GameObject));    
    }
    public void UseImpactGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, virtualCamera.position + virtualCamera.forward, virtualCamera.rotation);
        Explosion explosionScript = grenade.GetComponent<Explosion>();
        grenade.GetComponent<Rigidbody>().AddForce(virtualCamera.forward * explosionScript.grenadeThrowForce, ForceMode.Impulse);
    }

    public void SelfDestruct()
    {
        Destroy(this);
    }
}
