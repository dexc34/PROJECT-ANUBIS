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


    [Header ("Audio")]

    [SerializeField]
    private AudioClip shieldBreakSFX;


    [Header("Enemy Particles")]

    [Tooltip("DOES NOT need to be set if player")]
    [SerializeField] ParticleSystem damageParticle;
    [SerializeField] ParticleSystem deathParticle;

    //Script variables
    [HideInInspector] public float currentHealth;
    private bool isPlayer = false;
    private bool isEnemy = false;
    private bool isDestructible = false;

    //Required components
    private Destructibles destructibleScript;
    private AudioSource audioSource;

    //ANIMATION
    private Animator anim;
    [HideInInspector] public int isHitHash;

    private void Start() 
    {
        currentHealth = maxHealth;   
        audioSource = GetComponent<AudioSource>();
        if(gameObject.CompareTag("Player")) isPlayer = true;
        else if(gameObject.CompareTag("Hackable"))
        {
            isEnemy = true;
            isHitHash = Animator.StringToHash("isHit");
            anim = GetComponent<Animator>();
        }
        else if(gameObject.CompareTag("Destructible")) 
        {
            isDestructible = true;
            destructibleScript = GetComponent<Destructibles>();
        }

        UpdateHealth(this);
    }

    //quickly flashes gotHit for a frame then turns it off, for other scripts
    [HideInInspector] public bool gotHit;
    public void TakeDamage(float damageTaken)
    {
        StartCoroutine(CheckHit());
        if (isEnemy)
        {
            anim.SetTrigger(isHitHash);

            damageParticle.Play();
        }
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
        Debug.Log(gameObject.name +  " took damage (" + damageTaken + ")");
        if(currentHealth <= 0)
        {
            if(isPlayer) PlayerDie();
            //else if (isEnemy) EnemyDie(); //note: handeled in the enemies respective state machine instead -lucas
            else if (isDestructible) DestroyDestructible();
        }
    }

    public void Heal(float amountToHeal)
    {
        float previousHealth = currentHealth;
        currentHealth += amountToHeal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Before: " + previousHealth + "| After: " + currentHealth);
    }

    IEnumerator CheckHit()
    {
        gotHit = true;
        yield return null;
        gotHit = false;
    }

    private IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(shieldBreakIframes);
        audioSource.PlayOneShot(shieldBreakSFX);
        hasAntivirusShield = false;
    }

    private void PlayerDie()
    {
        Debug.Log("You died");
    }

    [HideInInspector] public bool enemyIsDead = false;
    public void EnemyDie()
    {
        enemyIsDead = true;
        deathParticle.Play();
    }

    private void DestroyDestructible()
    {
        destructibleScript.Destroyed();
    }

    public void UpdateHealth(Health healthScriptToPullFrom)
    {
        maxHealth = healthScriptToPullFrom.maxHealth;
        currentHealth = healthScriptToPullFrom.currentHealth;
    }
}

