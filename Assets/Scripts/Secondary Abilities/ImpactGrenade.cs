using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ImpactGrenade : MonoBehaviour
{
    [HideInInspector] public float cooldown = 8;
    private GameObject grenadePrefab;
    private Transform virtualCamera;

    private void Start() 
    {
        virtualCamera = GetComponentInChildren<CameraMove>().gameObject.transform;
#if UNITY_EDITOR
        grenadePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Projectiles/Grenade.prefab", typeof(GameObject));
#endif
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
