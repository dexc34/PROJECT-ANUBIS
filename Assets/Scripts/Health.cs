using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //Editor tools
    [Header ("Antivirus shield settings")]
    [SerializeField]
    [Tooltip ("If ticked as true will activate antivirus shield")]
    public bool hasAntivirusShield;

    [SerializeField]
    [Tooltip ("How much damage the antivirus shield can take")]
    private float antivirusShieldEnergy;

    [SerializeField]
    [Tooltip ("How many seconds of invulnerability the enemy has on shield break")]
    private float shieldBreakIframes;

    [Header ("Health settings")]
    [SerializeField]
    [Tooltip ("Amount of health object starts with")]
    public int maxHealth = 100;

    //Script variables
    [HideInInspector] public int currentHealth;
    private bool isPlayer = false;
    private bool isEnemy = false;
    private bool isDestructible = false;

    //Required components
    private Destructibles destructibleScript;

    private void Start() 
    {
        currentHealth = maxHealth;   
        if(gameObject.CompareTag("Player")) isPlayer = true;
        else if(gameObject.CompareTag("Hackable")) isEnemy = true;
        else if(gameObject.CompareTag("Destructible")) 
        {
            isDestructible = true;
            destructibleScript = GetComponent<Destructibles>();
        }
    }
    public void TakeDamage(int damageTaken)
    {
        if(hasAntivirusShield)
        {
            antivirusShieldEnergy -= damageTaken;
            if(antivirusShieldEnergy <= 0)
            {
                StartCoroutine("DeactivateShield");
            }
            return;
        }

        currentHealth -= damageTaken;
        Debug.Log("Take damage");
        if(currentHealth <= 0)
        {
            if(isPlayer) PlayerDie();
            else if (isEnemy) EnemyDie();
            else if (isDestructible) DestroyDestructible();
        }
    }

    private IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(shieldBreakIframes);
        hasAntivirusShield = false;
    }

    private void PlayerDie()
    {
        Debug.Log("You died");
    }

    private void EnemyDie()
    {
        Destroy(gameObject);
    }

    private void DestroyDestructible()
    {
        destructibleScript.Destroyed();
    }
}

