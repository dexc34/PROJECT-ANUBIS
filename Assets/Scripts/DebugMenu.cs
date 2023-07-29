using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    private GameObject debugMenu;

    [Header ("Dropdowns")]

    [SerializeField]
    private TMP_Dropdown primaryWeaponDropdown;

    [SerializeField]
    private TMP_Dropdown meleeAttackDropdown;

    [SerializeField]
    private TMP_Dropdown secondaryAbilityDropdown;


    [Header ("Toggles")]

    [SerializeField]
    private Toggle infiniteAmmoToggle;

    [SerializeField]
    private Toggle instantReloadToggle;

    [SerializeField]
    private Toggle turboFireRateToggle;

    [SerializeField]
    private Toggle automaticWeaponsToggle;

    [SerializeField]
    private Toggle abilityCooldownToggle;

    [SerializeField]
    private Toggle fpsCounterToggle;

    [SerializeField]
    private GameObject fpsCounter;

    //Script variables
    private bool menuOpen = false;

    //Required components
    private PlayerInput playerInputHandler;
    private Gun playerGunScript;
    private Melee playerMeleeScript;
    private SecondaryAbility playerSecondaryAbilityScript;

    private void Awake() 
    {
        GameObject player = GameObject.Find("Player");
        playerInputHandler = player.GetComponent<PlayerInput>();
        playerGunScript = player.GetComponent<Gun>();
        playerMeleeScript = player.GetComponent<Melee>();
        playerSecondaryAbilityScript = player.GetComponent<SecondaryAbility>();

        SetUiElements();
    }

    private void Start() 
    {
        CloseMenu();
    }

    public void CallDebugMenu(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        if(!menuOpen) OpenMenu();
        else CloseMenu();
    }
    private void OpenMenu()
    {
        menuOpen = true;
        playerInputHandler.SwitchCurrentActionMap("Menu");
        Time.timeScale = 0;
        debugMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    private void CloseMenu()
    {
        menuOpen = false;
        playerInputHandler.SwitchCurrentActionMap("Combat");
        Time.timeScale = 1;
        debugMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    //Sets dropdown values, creates use for toggles
    private void SetUiElements()
    {
        infiniteAmmoToggle.isOn = playerGunScript.infiniteAmmo;
        instantReloadToggle.isOn = playerGunScript.instantReload;
        turboFireRateToggle.isOn = playerGunScript.turboFireRate;
        automaticWeaponsToggle.isOn = playerGunScript.automatic;
        abilityCooldownToggle.isOn = playerSecondaryAbilityScript.overrideCooldown;

        fpsCounterToggle.isOn = fpsCounter.active;

    }

    //-------------------------UI Functions-----------------------------------------------------------------

    public void PrimaryWeaponDropdownChange(int index)
    {
        switch(index)
        {
            case 0:
                playerGunScript.gunType = GunTypeDropdownOptions.Pistol;
                playerGunScript.UpdateGunStats(playerGunScript);
                playerGunScript.RecoverAmmo(playerGunScript.totalAmmo);
                break;
            case 1:
                playerGunScript.gunType = GunTypeDropdownOptions.Shotgun;
                playerGunScript.UpdateGunStats(playerGunScript);
                playerGunScript.RecoverAmmo(playerGunScript.totalAmmo);
                break;
            case 2:
                playerGunScript.gunType = GunTypeDropdownOptions.AssaultRifle;
                playerGunScript.UpdateGunStats(playerGunScript);
                playerGunScript.RecoverAmmo(playerGunScript.totalAmmo);
                break;
            case 3:
                playerGunScript.gunType = GunTypeDropdownOptions.RocketLauncher;
                playerGunScript.UpdateGunStats(playerGunScript);
                playerGunScript.RecoverAmmo(playerGunScript.totalAmmo);
                break;
            case 4:
                playerGunScript.gunType = GunTypeDropdownOptions.SniperRifle;
                playerGunScript.UpdateGunStats(playerGunScript);
                playerGunScript.RecoverAmmo(playerGunScript.totalAmmo);
                break;
            case 5:
                playerGunScript.gunType = GunTypeDropdownOptions.Staff;
                playerGunScript.UpdateGunStats(playerGunScript);
                playerGunScript.RecoverAmmo(playerGunScript.totalAmmo);
                break;
            case 6:
                playerGunScript.gunType = GunTypeDropdownOptions.Melee;
                playerGunScript.UpdateGunStats(playerGunScript);
                break;
        }
    }

    public void MeleeDropdownChange(int index)
    {
        switch(index)
        {
            case 0:
                playerMeleeScript.meleeType = MeleeTypeDropdown.SingleSword;
                playerMeleeScript.UpdateMelee(playerMeleeScript);
                break;
            case 1:
                playerMeleeScript.meleeType = MeleeTypeDropdown.DualWieldedChainSwords;
                playerMeleeScript.UpdateMelee(playerMeleeScript);
                break;
            case 2:
                playerMeleeScript.meleeType = MeleeTypeDropdown.Fist;
                playerMeleeScript.UpdateMelee(playerMeleeScript);
                break;
            case 3:
                playerMeleeScript.meleeType = MeleeTypeDropdown.Kick;
                playerMeleeScript.UpdateMelee(playerMeleeScript);
                break;
        }
    }

    public void SecondaryAbilityDropdownChange(int index)
    {
        switch(index)
        {
            case 0:
                playerSecondaryAbilityScript.secondaryType = SecondaryDropdownOptions.ImpactGrenade;
                playerSecondaryAbilityScript.UpdateSecondary(playerSecondaryAbilityScript);
                break;
            case 1:
                playerSecondaryAbilityScript.secondaryType = SecondaryDropdownOptions.Barrage;
                playerSecondaryAbilityScript.UpdateSecondary(playerSecondaryAbilityScript);
                break;
            case 2:
                playerSecondaryAbilityScript.secondaryType = SecondaryDropdownOptions.WrathOfRa;
                playerSecondaryAbilityScript.UpdateSecondary(playerSecondaryAbilityScript);
                break;
            case 3:
                playerSecondaryAbilityScript.secondaryType = SecondaryDropdownOptions.CloserToThePrey;
                playerSecondaryAbilityScript.UpdateSecondary(playerSecondaryAbilityScript);
                break;
            case 4:
                playerSecondaryAbilityScript.secondaryType = SecondaryDropdownOptions.Leap;
                playerSecondaryAbilityScript.UpdateSecondary(playerSecondaryAbilityScript);
                break;
        }
    }

    public void InfiniteAmmoToggle(bool toggleValue)
    {
        playerGunScript.infiniteAmmo = toggleValue;
    }

    public void InstantReloadToggle(bool toggleValue)
    {
        playerGunScript.instantReload = toggleValue;
    }

    public void TurboFireRateToggle(bool toggleValue)
    {
        playerGunScript.turboFireRate = toggleValue;
    }
    public void AutomaticWeaponsToggle(bool toggleValue)
    {
        playerGunScript.automatic = toggleValue;
        playerGunScript.isAutomatic = toggleValue;
    }

    public void AbilityCooldownToggle(bool toggleValue)
    {
        playerSecondaryAbilityScript.overrideCooldown = toggleValue;
        playerSecondaryAbilityScript.ResetCooldown();
    }

    public void LoadLevel(int index)
    {
        CloseMenu();
        SceneManager.LoadScene(index);
    }

    public void FPSToggle(bool toggleValue)
    {
        fpsCounter.SetActive(toggleValue);
    }
}
