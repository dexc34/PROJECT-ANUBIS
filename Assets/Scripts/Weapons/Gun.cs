using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum GunTypeDropdownOptions{Pistol, Shotgun, AssaultRifle, RocketLauncher, SniperRifle, Staff, Melee};

public class Gun : MonoBehaviour
{
    //Editor tools
    public GunTypeDropdownOptions gunType = new GunTypeDropdownOptions();  

    [Header ("Cheats")]

    [SerializeField]
    private bool infiteAmmo;

    [SerializeField]
    private bool instantReload;

    [SerializeField]
    private bool turboFireRate;

    [SerializeField]
    private bool automatic;

    //Inherited variables from scriptable
    private int damagePerBullet;
    private float bulletSpeed;
    private float bulletLifetime;
    private float criticalMultiplier;
    [HideInInspector] public bool isAutomatic;
    private float fireRate;
    private float reloadSpeed; 
    [HideInInspector] public bool individualBulletReload;
    private float individualBulletReloadSpeed;
    [HideInInspector] public int totalAmmo;
    private int magazineSize;
    private int bulletPenetrationAmount;
    private int bulletsPerBurst;
    private Vector2[] bulletSpread;
    private float zSpread = 5;

    private GameObject weaponModelPrefab;
    private GameObject newGun, oldGun;

    private Sprite crosshairSprite;
    private string fireModeString;

    private GunAudio gunAudioScript;
    private bool needsCock;
    private GameObject bulletPrefab;
    private GameObject bulletMarkPrefab;
    
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

    [SerializeField]
    [Tooltip("Must only be specified if this is the player")]
    private Camera mainCam;
    //------------------------------------------------------CAN PROBABLY GET THESE VIA SCRIPT ^^


    //Script variables
    [HideInInspector] public bool primaryIsMelee = false;
    [HideInInspector] public int currentAmmo;
    private int currentMagazine;
    private int ammoToDisplay;
    [HideInInspector] public bool canFire = true;
    private float shootCooldown;
    private bool isEnemy = true;
    private int layerToApplyToBullet;
    [HideInInspector] public bool interruptFire = false;
    private bool isReloading = false;
    [HideInInspector] public Coroutine reloadCoroutine;


    //Required components
    private GameObject playerPos;
    private Transform virtualCamera;   

    public WeaponViewmodelAnimations viewModelScript;
    private ParticleSystem muzzleParticle;
    private GunTemplate gunScriptable;
    private AudioSource shootAudioSource;
    private Camera viewmodelCam;
    private LayerMask bulletHoleLayersToCheck;

    void Start()
    {
        playerPos = GameObject.Find("Player");
        shootAudioSource = GetComponent<AudioSource>();
        gunScriptable = (GunTemplate) Resources.Load(gunType.ToString());
        currentAmmo = gunScriptable.totalAmmo;

        //Set layer mask
        bulletHoleLayersToCheck = Physics.AllLayers;
        bulletHoleLayersToCheck &= ~(1 << 3);
        bulletHoleLayersToCheck &= ~(1 << 7);
        bulletHoleLayersToCheck &= ~(1 << 9);
        bulletHoleLayersToCheck &= ~(1 << 10);
        bulletHoleLayersToCheck &= ~(1 << 13);
        bulletHoleLayersToCheck &= ~(1 << 15);


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
        if(!canFire || currentAmmo <= 0 || interruptFire) return;

        //Interrupt individual bullet reload
        if(reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        isReloading = false;
        
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

            //Get bullet hole position
            Physics.Raycast(virtualCamera.position + virtualCamera.forward, bulletFinalDestination, out RaycastHit hitInfo, bulletSpeed * bulletLifetime, bulletHoleLayersToCheck);
            Vector3 holePosition = hitInfo.point + (hitInfo.normal * 0.1f);
            Quaternion holeRotation = Quaternion.FromToRotation(Vector3.up,  hitInfo.normal);

            bullet.GetComponent<Rigidbody>().AddForce(bulletFinalDestination * bulletSpeed/zSpread, ForceMode.Impulse);
            //Does not apply to weapons that don't shoot bullets (eg. rocket launcher)
            if(bullet.GetComponent<Bullets>() != null)
            {
                Bullets bulletScript = bullet.GetComponent<Bullets>();

                bulletScript.damage = damagePerBullet;
                bulletScript.damageMultipler = criticalMultiplier;
                bulletScript.penetrationAmmout = bulletPenetrationAmount;
                bulletScript.destroyTime = bulletLifetime;
                bulletScript.GetBulletHoleInfo(bulletMarkPrefab, holePosition, holeRotation);
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
            Reload();
        }
        //If not necessary wait for the shoot cooldown
        else
        {
            StartCoroutine("ShootCooldown");
        }
    }

    private IEnumerator ShootCooldown()
    {
        if(turboFireRate) shootCooldown = 0.1f;

        if (needsCock)
            gunAudioScript.PlayCockClip(shootAudioSource);
        yield return new WaitForSeconds(shootCooldown);
        canFire = true;
    }

    public void Reload()
    {
        if(individualBulletReload)
            {
                reloadCoroutine = StartCoroutine("InterruptableReload");
            }
        else StartCoroutine("FullReload");
    }

    private IEnumerator FullReload()
    {
        if(currentMagazine == magazineSize || ammoToDisplay <= 0 || isReloading) yield break;

        isReloading = true;
        StopCoroutine("ShootCooldown");
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
        isReloading = false;
    }

    private IEnumerator InterruptableReload()
    {
        if(currentMagazine == magazineSize || ammoToDisplay <= 0 || isReloading) yield break;

        isReloading = true;
        StopCoroutine("ShootCooldown");
        canFire = false;

        if(instantReload) individualBulletReloadSpeed = 0.1f;

        for(int i = currentMagazine; i < magazineSize; i ++)
        {

            yield return new WaitForSeconds(individualBulletReloadSpeed);

            currentMagazine ++;
            if(!infiteAmmo) ammoToDisplay --;

            //Update UI
            currentAmmoText.text = currentMagazine.ToString();
            reserveAmmoText.text = ammoToDisplay.ToString();


            gunAudioScript.PlayReloadClip(shootAudioSource);

            canFire = true;
            if(currentMagazine >= magazineSize || currentMagazine >= currentAmmo || ammoToDisplay <= 0)
            {
                isReloading = false;
                yield break;
            }
        }
    }

    public void RecoverAmmo(int ammoToRecover)
    {
        currentAmmo += ammoToRecover;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, totalAmmo);
        ammoToDisplay = currentAmmo - currentMagazine;
        reserveAmmoText.text = ammoToDisplay.ToString();

        if(currentMagazine <= 0) Reload();
    }


    //Used when hacking a new body, update gun stats to match the enemy's gun
    public void UpdateGunStats(Gun gunScriptToPullFrom)
    {
        interruptFire = true;

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
        if(automatic) isAutomatic = true;
        fireRate = gunScriptable.fireRate;
        reloadSpeed = gunScriptable.reloadSpeed;
        individualBulletReload = gunScriptable.individualBulletReload;
        individualBulletReloadSpeed = gunScriptable.individualBulletReloadSpeed;
        totalAmmo = gunScriptable.totalAmmo;
        magazineSize = gunScriptable.magazineSize;
        bulletPenetrationAmount = gunScriptable.bulletPenetrationAmount;
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
        bulletMarkPrefab = gunScriptable.bulletMarkPrefab;

        //Apply internally tracked stats
        currentAmmo = gunScriptToPullFrom.currentAmmo;
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

        interruptFire = false;

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

        if (gunType.ToString() == "Melee") return;
        //destroys the previous weapon model
        //if (newGun != null)
        //{
        //    oldGun = newGun;
        //    Destroy(oldGun);
        //}
        GameObject weaponHolder = transform.GetComponentInChildren<CameraMove>().gameObject.transform.Find("Weapon Holder").gameObject;
        newGun = Instantiate(weaponModelPrefab, weaponHolder.transform.position, weaponHolder.transform.rotation);
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

            //Set bullet hole info
            Physics.Raycast(enemyBulletSpawn.transform.position + enemyBulletSpawn.transform.forward, bulletFinalDestination, out RaycastHit hitInfo, bulletSpeed * bulletLifetime, bulletHoleLayersToCheck);
            Vector3 holePosition = hitInfo.point + (hitInfo.normal * 0.1f);
            Quaternion holeRotation = Quaternion.FromToRotation(Vector3.up,  hitInfo.normal);

            bullet.GetComponent<Rigidbody>().AddForce(bulletFinalDestination * bulletSpeed/zSpread, ForceMode.Impulse);
            //Does not apply to weapons that don't shoot bullets (eg. rocket launcher)
            if(bullet.GetComponent<Bullets>() != null)
            {
                bullet.GetComponent<Bullets>().damage = damagePerBullet;
                bullet.GetComponent<Bullets>().damageMultipler = criticalMultiplier;
                bullet.GetComponent<Bullets>().destroyTime = bulletLifetime;
                bullet.GetComponent<Bullets>().GetBulletHoleInfo(bulletMarkPrefab, holePosition, holeRotation);
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
