using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Gun", menuName = "Gun")]
public class GunTemplate : ScriptableObject
{
    [Header ("Stats")]

    public float bulletSpeed;

    [Tooltip ("How long bullets will exist before dissapearing in seconds")]
    public float bulletLifetime;

    [Tooltip ("Damage multiplier when hitting a critical point (eg. headshots)")] 
    public float criticalMultiplier;
        
    [Tooltip ("Can the player hold to shoot?")]
    public bool isAutomatic = false;

    [Tooltip ("Meassured in bursts fired per second")] 
    public float fireRate;

    [Tooltip ("How long it takes to reload, meassured in seconds")] 
    public float reloadSpeed; 

    [Tooltip ("Does the gun reload all bullets all at once, or one at a time, makes reload interruptable")]
    public bool individualBulletReload;

    [Tooltip ("How long it takes to reload an individual bullet, ignores reload speed, only relevant if previous bool is true")]
    public float individualBulletReloadSpeed;

    [Tooltip ("Total amount of bullets available")] 
    public int totalAmmo;

    [Tooltip ("How many bullets can be fired before needing to reload")] 
    public int magazineSize;


    [Header ("Bullet Properties")]

    [Tooltip ("How much damage an indiviual hit deals")]
    public int damagePerBullet;

    [Tooltip ("How many objects a bullet can go through before disappearing")]
    public int bulletPenetrationAmount;

    [Tooltip ("How many bullets come out of the gun on shoot")] 
    public int bulletsPerBurst;

    [Tooltip ("Array size should match the amount of Bullets Per Burst (0, 0, 0 means it will have no spread)")] 
    public Vector2[] bulletSpread;

    [Tooltip ("Amount of units it takes for a bullet to reach its spread pattern")]
    public float zSpread = 5;



    [Header("Visuals")]
    public GameObject weaponModelPrefab;
    public GameObject bulletMarkPrefab;


    [Header ("UI")]

    [Tooltip ("Sprite of the corresponding weapon crosshair")]
    public Sprite crosshairSrpite;

    [Tooltip ("What kind of fire mode the gun has (eg. auto, semi, pump, etc.)")]
    public string fireModeString;


    [Header ("Audio")]
    
    [Tooltip("Gun audio scriptable object")]
    public GunAudio gunAudioScript;

    [Tooltip("Does the gun need to be cocked?")]
    public bool needsCock;



    [Header ("Objects")]

    [Tooltip ("Bullet prefab goes here")] 
    public GameObject bulletPrefab;
}
