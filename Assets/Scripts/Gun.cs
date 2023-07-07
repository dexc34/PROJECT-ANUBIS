using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

    public enum SecondaryDropdownOptions{ImpactGrenade, Barrage, WrathOfRa, CloserToThePrey};
public class Gun : MonoBehaviour
{
    //Editor tools
    private enum GunTypeDropdownOptions{Pistol, Shotgun, AssaultRifle, RocketLauncher, Staff};
    [SerializeField]
    GunTypeDropdownOptions gunType = new GunTypeDropdownOptions();


    [SerializeField]
    SecondaryDropdownOptions secondaryType = new SecondaryDropdownOptions();

    [SerializeField] 
    private bool isEnemy = true;

    [SerializeField]
    private GameObject weaponModelPrefab;

    [SerializeField] 
    private float bulletSpeed;

    [SerializeField]
    [Tooltip ("How long bullets will exist before dissapearing in seconds")]
    private float bulletLifetime = 5;

    [SerializeField] 
    private int damagePerBullet;

    [SerializeField] 
    [Tooltip ("Damage multiplier when hitting a critical point (eg. headshots)")] 
    private float criticalMultiplier;

    [SerializeField]
    [Tooltip ("Can the player hold to shoot?")]
    public bool isAutomatic = false;

    [SerializeField] 
    [Tooltip ("Meassured in bursts fired per second")] 
    private float fireRate;

    [SerializeField] 
    [Tooltip ("How long it takes to reload, meassured in seconds")] 
    private float reloadSpeed; 

    [SerializeField] 
    [Tooltip ("Total amount of bullets available")] 
    private int totalAmmo;

    [SerializeField] 
    [Tooltip ("How many bullets can be fired before needing to reload")] 
    private int magazineSize;

    [SerializeField] 
    [Tooltip ("How many bullets come out of the gun on shoot")] 
    private int bulletsPerBurst;

    [SerializeField] 
    [Tooltip ("Array size should match the amount of Bullets Per Burst (0, 0, 0 means it will have no spread)")] 
    private Vector2[] bulletSpread;

    [SerializeField]
    [Tooltip ("Amount of units it takes for a bullet to reach its spread pattern")]
    private float zSpread = 5;

    [SerializeField] 
    [Tooltip ("Bullet prefab goes here")] 
    private GameObject bulletPrefab;

    [Header ("UI")]

    [SerializeField] 
    private Text currentAmmoText;
    [SerializeField]
    private Text reserveAmmoText;
    [SerializeField]
    private Text fireModeText;
    [SerializeField]
    private string fireModeString;

    [SerializeField] 
    [Tooltip ("Must only be specified if this is the player")]
    private Image crosshairUiElement;
    [SerializeField] 
    [Tooltip ("Sprite of the corresponding weapon")]
    private Sprite crosshair;

    //gets viewmodel camera for the gun
    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private Camera viewmodelCam;
    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private Camera mainCam;

    //Gets sound effects for the guns
    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private GunAudio gunAudioScript;
    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private AudioSource shootAudioSource;

    //Script variables
    private int currentAmmo;
    private int currentMagazine;
    private int ammoToDisplay;
    [HideInInspector] public bool canFire = true;
    private float shootCooldown;

    [SerializeField]
    [Tooltip("Does the gun need to be cocked?")]
    bool needsCock;

    //Required components
    private GameObject playerPos;
    private Transform virtualCamera;   
    private SecondaryAbility secondaryAbilityScript;
    public WeaponViewmodelAnimations viewModelScript;
    public ParticleSystem muzzleParticle;

    void Start()
    {
        playerPos = GameObject.Find("Player");
        currentAmmo = totalAmmo;
        shootAudioSource = GetComponent<AudioSource>();
        UpdateGunStats(this);
    }

    //----------------------------------Player Functions-----------------------------------------------------------------------------------
    public void FireBullet()
    {
        if(!canFire || currentAmmo <= 0) return;
        
        canFire = false;
        currentAmmo --;
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
//        isShoothing = false;

        //Apply serializable gun stats
        gunType = gunScriptToPullFrom.gunType;
        secondaryType = gunScriptToPullFrom.secondaryType;
        weaponModelPrefab = gunScriptToPullFrom.weaponModelPrefab;
        bulletSpeed = gunScriptToPullFrom.bulletSpeed;
        bulletLifetime = gunScriptToPullFrom.bulletLifetime;
        damagePerBullet = gunScriptToPullFrom.damagePerBullet;
        criticalMultiplier = gunScriptToPullFrom.criticalMultiplier;
        isAutomatic = gunScriptToPullFrom.isAutomatic;
        fireRate = gunScriptToPullFrom.fireRate;
        reloadSpeed = gunScriptToPullFrom.reloadSpeed;
        totalAmmo = gunScriptToPullFrom.totalAmmo;
        magazineSize = gunScriptToPullFrom.magazineSize;
        bulletPrefab = gunScriptToPullFrom.bulletPrefab;
        bulletsPerBurst = gunScriptToPullFrom.bulletsPerBurst;
        bulletSpread = gunScriptToPullFrom.bulletSpread;

        //Apply internally tracked stats
        currentAmmo = gunScriptToPullFrom.currentAmmo;

        //Make sure player can fire again if last body they hacked ran out of bullets
        if(currentAmmo <= 0) canFire = false;
        else canFire = true;

        if(currentAmmo <= gunScriptToPullFrom.magazineSize)
        {
            currentMagazine = currentAmmo;
            ammoToDisplay = 0;            
        }
        else
        {
            currentMagazine = magazineSize;
            ammoToDisplay = currentAmmo - magazineSize;
        }

        shootCooldown = 1/fireRate;

        if(isEnemy) return;

        //Only run if script is on the player

        //Update camera       
        var cameraData = mainCam.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Remove(viewmodelCam);

        viewmodelCam = transform.GetComponentInChildren<CameraMove>().GetComponentInChildren<Camera>();
        cameraData.cameraStack.Insert(0, viewmodelCam);

        virtualCamera = GetComponentInChildren<CameraMove>().gameObject.transform;

        //Update secondary ability script
        secondaryAbilityScript = GetComponent<SecondaryAbility>();
        secondaryAbilityScript.UpdateSecondary(secondaryType);

        //Update UI
        currentAmmoText.text = currentMagazine.ToString();
        reserveAmmoText.text = ammoToDisplay.ToString();
        //sets the string for fire mode 
        fireModeText.text = gunScriptToPullFrom.fireModeString;

        //sets new sounds for new gun
        gunAudioScript = gunScriptToPullFrom.gunAudioScript;

        //checks to see if new gun needs to be cocked
        needsCock = gunScriptToPullFrom.needsCock;

        crosshair = gunScriptToPullFrom.crosshair;
        crosshairUiElement.sprite = crosshair;

        //Update gun model
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
