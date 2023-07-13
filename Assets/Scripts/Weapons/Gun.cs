using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum GunTypeDropdownOptions{Pistol, Shotgun, AssaultRifle, RocketLauncher, Staff, Melee};

public class Gun : MonoBehaviour
{
    //Editor tools
    public GunTypeDropdownOptions gunType = new GunTypeDropdownOptions();  

    [Header ("Cheats")]

    [SerializeField]
    private bool infiteAmmo;

    [SerializeField]
    private bool instantReload;

    //Inherited variables from scriptable
    private int damagePerBullet;
    private float bulletSpeed;
    private float bulletLifetime;
    private float criticalMultiplier;
    [HideInInspector] public bool isAutomatic;
    private float fireRate;
    private float reloadSpeed; 
    private int totalAmmo;
    private int magazineSize;
    private int bulletsPerBurst;
    private Vector2[] bulletSpread;
    private float zSpread = 5;

    private GameObject weaponModelPrefab;

    private Sprite crosshairSprite;
    private string fireModeString;

    private GunAudio gunAudioScript;
    private bool needsCock;
    private GameObject bulletPrefab;
    
    //------------------------------------------------------CAN PROBABLY GET THESE VIA SCRIPT vv

    [Header ("UI")]

    [SerializeField] 
    private Text currentAmmoText;
    [SerializeField]
    private Text reserveAmmoText;
    [SerializeField]
    private Text fireModeText;

    [SerializeField] 
    [Tooltip ("Must only be specified if this is the player")]
    private Image crosshairUiElement;

    //gets viewmodel camera for the gun
    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private Camera viewmodelCam;
    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private Camera mainCam;
    //------------------------------------------------------CAN PROBABLY GET THESE VIA SCRIPT ^^


    //Script variables
    [HideInInspector] public bool primaryIsMelee = false;
    private int currentAmmo;
    private int currentMagazine;
    private int ammoToDisplay;
    [HideInInspector] public bool canFire = true;
    private float shootCooldown;
    private bool isEnemy = true;
    private int layerToApplyToBullet;


    //Required components
    private GameObject playerPos;
    private Transform virtualCamera;   

    public WeaponViewmodelAnimations viewModelScript;
    public ParticleSystem muzzleParticle;
    private GunTemplate gunScriptable;
    private AudioSource shootAudioSource;

    void Start()
    {
        playerPos = GameObject.Find("Player");
        shootAudioSource = GetComponent<AudioSource>();
        gunScriptable = (GunTemplate) Resources.Load(gunType.ToString());
        currentAmmo = gunScriptable.totalAmmo;

        if(gameObject.CompareTag("Player")) 
        {
            isEnemy = false;
            layerToApplyToBullet = 3;
        }
        else
        {
            isEnemy = true;
            layerToApplyToBullet = 9;
        }
        UpdateGunStats(this);
    }

    //----------------------------------Player Functions-----------------------------------------------------------------------------------
    public void FireBullet()
    {
        if(!canFire || currentAmmo <= 0) return;
        
        canFire = false;
        if(!infiteAmmo)currentAmmo --;
        currentMagazine --;

        gunAudioScript.PlayShootClip(shootAudioSource);
        currentAmmoText.text = currentMagazine.ToString();
        reserveAmmoText.text = ammoToDisplay.ToString();

        //Puts recoil on viewmodel animation
        viewModelScript.Recoil();

        //Fire a specified amount of bullets per burst
        for(int i = 0; i < bulletsPerBurst; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, virtualCamera.position + virtualCamera.forward, virtualCamera.rotation);
            bullet.layer = layerToApplyToBullet;
            Vector3 bulletFinalDestination = (virtualCamera.right * bulletSpread[i].x) + (virtualCamera.up * bulletSpread[i].y) + (virtualCamera.forward * zSpread);
            bullet.GetComponent<Rigidbody>().AddForce(bulletFinalDestination * bulletSpeed/zSpread, ForceMode.Impulse);
            //Does not apply to weapons that don't shoot bullets (eg. rocket launcher)
            if(bullet.GetComponent<Bullets>() != null)
            {
                bullet.GetComponent<Bullets>().damage = damagePerBullet;
                bullet.GetComponent<Bullets>().destroyTime = bulletLifetime;
            }
        }

        //Render Muzzle Particle
        muzzleParticle.Clear();
        muzzleParticle.Play();

        //Don't set canFire to true if out of ammo
        if(currentAmmo == 0)
        {
            return;
        }

        //Reload if necessary
        if(currentMagazine <= 0)
        {
            canFire = true;
            StartCoroutine("Reload");
        }
        //If not necessary wait for the shoot cooldown
        else
        {
            StartCoroutine("ShootCooldown");
        }
    }

    private IEnumerator ShootCooldown()
    {
        if (needsCock)
            gunAudioScript.PlayCockClip(shootAudioSource);
        yield return new WaitForSeconds(shootCooldown);
        canFire = true;
    }

    public IEnumerator Reload()
    {
        if(currentMagazine == magazineSize || ammoToDisplay <= 0 ||!canFire) yield break;
        canFire = false;

        gunAudioScript.PlayReloadClip(shootAudioSource);

        if(instantReload) reloadSpeed = shootCooldown;

        yield return new WaitForSeconds(reloadSpeed);

        //If you have enough ammo for a full magazine, fill it up, and display remaining ammo
        if(magazineSize <= currentAmmo)
        {
            currentMagazine = magazineSize;
            ammoToDisplay = currentAmmo - magazineSize;
        }
        //If not, magazine size will be the amount of remaining ammo, and your ammo reserve gets set to 0
        else
        {
            currentMagazine = currentAmmo;
            ammoToDisplay = 0;
        }
        currentAmmoText.text = currentMagazine.ToString();
        reserveAmmoText.text = ammoToDisplay.ToString();
        canFire = true;
    }


    //Used when hacking a new body, update gun stats to match the enemy's gun
    public void UpdateGunStats(Gun gunScriptToPullFrom)
    {
        gunType = gunScriptToPullFrom.gunType;
        gunScriptable = (GunTemplate) Resources.Load(gunType.ToString());

        if(gunType.ToString() == "Melee") primaryIsMelee = true;
        else primaryIsMelee = false;

        //Inhertit scriptable variables

        //Stats
        damagePerBullet = gunScriptable.damagePerBullet;
        bulletSpeed = gunScriptable.bulletSpeed;
        bulletLifetime = gunScriptable.bulletLifetime;
        criticalMultiplier = gunScriptable.criticalMultiplier;
        isAutomatic = gunScriptable.isAutomatic;
        fireRate = gunScriptable.fireRate;
        reloadSpeed = gunScriptable.reloadSpeed;
        totalAmmo = gunScriptable.totalAmmo;
        magazineSize = gunScriptable.magazineSize;
        bulletsPerBurst = gunScriptable.bulletsPerBurst;
        bulletSpread = gunScriptable.bulletSpread;
        zSpread = gunScriptable.zSpread;

        //Visuals
        weaponModelPrefab = gunScriptable.weaponModelPrefab;
        if(weaponModelPrefab == null) weaponModelPrefab = GetComponent<Melee>().meleeModel;

        //UI
        crosshairSprite = gunScriptable.crosshairSrpite;

        //Audio
        gunAudioScript = gunScriptable.gunAudioScript;
        needsCock = gunScriptable.needsCock;

        //Objects
        bulletPrefab = gunScriptable.bulletPrefab;

        //Apply internally tracked stats
        currentAmmo = gunScriptToPullFrom.currentAmmo;
        Debug.Log(currentAmmo);
        shootCooldown = 1/fireRate;

        //Make sure player can fire again if last body they hacked ran out of bullets
        if(currentAmmo <= 0) canFire = false;
        else canFire = true;

        if(currentAmmo <= magazineSize)
        {
            currentMagazine = currentAmmo;
            ammoToDisplay = 0;            
        }
        else
        {
            currentMagazine = magazineSize;
            ammoToDisplay = currentAmmo - magazineSize;
        }


        if(isEnemy) return;

        //Only run if script is on the player

        //Update camera       
        var cameraData = mainCam.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Remove(viewmodelCam);

        viewmodelCam = transform.GetComponentInChildren<CameraMove>().GetComponentInChildren<Camera>();
        cameraData.cameraStack.Insert(0, viewmodelCam);

        virtualCamera = GetComponentInChildren<CameraMove>().gameObject.transform;

        //Update UI
        currentAmmoText.text = currentMagazine.ToString();
        reserveAmmoText.text = ammoToDisplay.ToString();
        crosshairUiElement.sprite = crosshairSprite;
        fireModeText.text = gunScriptable.fireModeString;

        //Update gun model
        if(gunType.ToString() == "Melee") return;
        GameObject weaponHolder = transform.GetComponentInChildren<CameraMove>().gameObject.transform.Find("Weapon Holder").gameObject;
        GameObject newGun = Instantiate(weaponModelPrefab, weaponHolder.transform.position, weaponHolder.transform.rotation);
        newGun.transform.parent = weaponHolder.transform;
        viewModelScript = newGun.GetComponent<WeaponViewmodelAnimations>();
        muzzleParticle = newGun.GetComponentInChildren<ParticleSystem>();
    }

    //------------------------------------------------------Enemy functions------------------------------------------------------------------

    [Header("AI Settings")]
    [Tooltip("Where does the enemy shoot from?")]
    [SerializeField]GameObject enemyBulletSpawn;

    RaycastHit hit;
    public void EnemyShoot()
    {
        if(!canFire) return;
        
        canFire = false;

        gunAudioScript.PlayShootClip(shootAudioSource);

        //Fire a specified amount of bullets per burst
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            enemyBulletSpawn.transform.LookAt(playerPos.transform);
            GameObject bullet = Instantiate(bulletPrefab, enemyBulletSpawn.transform.position + enemyBulletSpawn.transform.forward, enemyBulletSpawn.transform.rotation);
            Vector3 bulletFinalDestination = (enemyBulletSpawn.transform.right * bulletSpread[i].x) + 
                (enemyBulletSpawn.transform.up * bulletSpread[i].y) + (enemyBulletSpawn.transform.forward * zSpread);
            bullet.GetComponent<Rigidbody>().AddForce(bulletFinalDestination * bulletSpeed/zSpread, ForceMode.Impulse);
            //Does not apply to weapons that don't shoot bullets (eg. rocket launcher)
            if(bullet.GetComponent<Bullets>() != null)
            {
                bullet.GetComponent<Bullets>().damage = damagePerBullet;
                bullet.GetComponent<Bullets>().destroyTime = bulletLifetime;
            }
        }

        //AI CURRENTLY DOES NOT HAVE MUZZLE PARTICLES, CONSIDER ADDING THIS LATER
        //Render Muzzle Particle
        //muzzleParticle.Clear();
        //muzzleParticle.Play();

        //Don't set canFire to true if out of ammo
        if(currentAmmo == 0)
        {
            return;
        }

        //If not necessary wait for the shoot cooldown
        else
        {
            StartCoroutine("ShootCooldown");
        }
    }
}
