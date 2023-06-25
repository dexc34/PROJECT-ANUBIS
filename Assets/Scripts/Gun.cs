using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    //Editor tools
    [SerializeField] private bool isEnemy = true;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float damagePerBullet;
    [SerializeField] [Tooltip ("Damage multiplier when hitting a critical point (eg. headshots)")] private float criticalMultiplier;
    [SerializeField] [Tooltip ("Meassured in bursts fired per second")] private float fireRate;
    [SerializeField] [Tooltip ("How long it takes to reload, meassured in seconds")] private float reloadSpeed; 
    [SerializeField] [Tooltip ("Total amount of bullets available")] private int totalAmmo;
    [SerializeField] [Tooltip ("How many bullets can be fired before needing to reload")] private int magazineSize;
    [SerializeField] [Tooltip ("How many bullets come out of the gun on shoot")] private int bulletsPerBurst;
    [SerializeField] [Tooltip ("Array size should match the amount of Bullets Per Burst (0, 0, 0 means it will have no spread)")] private Vector2[] bulletSpread;
    [SerializeField] [Tooltip ("Bullet prefab goes here")] private GameObject bulletPrefab;

    [Header ("UI")]
    [SerializeField] private Text ammoText;

    //Script variables
    private int currentAmmo;
    private int currentMagazine;
    private int ammoToDisplay;
    private bool canFire = true;
    private float shootCooldown;

    //Required components
    private Transform virtualCamera;   

    void Start()
    {
        UpdateGunStats(this);
    }

    //----------------------------------Player Functions-----------------------------------------------------------------------------------

    public void Shoot(InputAction.CallbackContext context)
    {
        if(!canFire || !context.performed) return;
        canFire = false;
        currentAmmo --;
        currentMagazine --;

        ammoText.text = currentMagazine.ToString() + "/" + ammoToDisplay.ToString();

        //Fire a specified amount of bullets per burst
        for(int i = 0; i < bulletsPerBurst; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, virtualCamera.position + virtualCamera.forward + (virtualCamera.right * bulletSpread[i].x) + (virtualCamera.up * bulletSpread[i].y) , virtualCamera.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(virtualCamera.forward* bulletSpeed, ForceMode.Impulse);
        }

        //Don't set canFire to true if out of ammo
        if(currentAmmo == 0)
        {
            return;
        }

        //Reload if necessary
        if(currentMagazine <= 0)
        {
            StartCoroutine("Reloading");
        }
        //If not necessary wait for the shoot cooldown
        else
        {
            StartCoroutine("ShootCooldown");
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if(!context.performed || currentMagazine == magazineSize || ammoToDisplay <= 0) return;
        StartCoroutine("Reloading");
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canFire = true;
        Debug.Log("Can shoot");
    }

    private IEnumerator Reloading()
    {
        Debug.Log("Reloading");
        canFire = false;

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
            Debug.Log(currentAmmo);
            currentMagazine = currentAmmo;
            ammoToDisplay = 0;
        }
        ammoText.text = currentMagazine.ToString() + "/" + ammoToDisplay.ToString();
        canFire = true;
    }


    //Used when hacking a new body, update gun stats to match the enemy's gun
    public void UpdateGunStats(Gun gunScriptToPullFrom)
    {
        //Make sure player can fire again if last body they hacked ran out of bullets
        canFire = true;

        //Apply serializable gun stats
        bulletSpeed = gunScriptToPullFrom.bulletSpeed;
        damagePerBullet = gunScriptToPullFrom.damagePerBullet;
        criticalMultiplier = gunScriptToPullFrom.criticalMultiplier;
        fireRate = gunScriptToPullFrom.fireRate;
        reloadSpeed = gunScriptToPullFrom.reloadSpeed;
        totalAmmo = gunScriptToPullFrom.totalAmmo;
        magazineSize = gunScriptToPullFrom.magazineSize;
        bulletsPerBurst = gunScriptToPullFrom.bulletsPerBurst;
        bulletSpread = gunScriptToPullFrom.bulletSpread;

        //Apply internally tracked stats
        currentAmmo = totalAmmo;
        currentMagazine = magazineSize;
        ammoToDisplay = totalAmmo - magazineSize;
        shootCooldown = 1/fireRate;

        if(isEnemy) return;

        //Update UI
        ammoText.text = currentMagazine.ToString() + "/" + ammoToDisplay.ToString();

        //Update camera
        virtualCamera = GetComponentInChildren<CameraMove>().gameObject.transform;
    }

    //------------------------------------------------------Enemy functions------------------------------------------------------------------
    public void EnemyShoot()
    {
        canFire = false;
        currentMagazine --;

        //Fire a specified amount of bullets per burst
        for(int i = 0; i < bulletsPerBurst; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, virtualCamera.position + virtualCamera.forward + (virtualCamera.right * bulletSpread[i].x) + (virtualCamera.up * bulletSpread[i].y) , virtualCamera.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(virtualCamera.forward* bulletSpeed, ForceMode.Impulse);
        }

        //Reload if necessary
        if(currentMagazine <= 0)
        {
            StartCoroutine("EnemyReload");
        }
        //If not necessary wait for the shoot cooldown
        else
        {
            StartCoroutine("EnemyShootCooldown");
        }
    }

    public IEnumerator EnemyReload()
    {
        canFire = false;

        yield return new WaitForSeconds(reloadSpeed);

        currentMagazine = magazineSize;
        canFire = true;
    }

    public IEnumerator EnemyShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canFire = true;
    }
}
