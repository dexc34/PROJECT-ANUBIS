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

    [Header("Enemy Particles")]
    [Tooltip("DOES NOT need to be set if player")]
    [SerializeField] ParticleSystem damageParticle;
    [SerializeField] ParticleSystem deathParticle;

    //Script variables
    [HideInInspector] public int currentHealth;
    private bool isPlayer = false;
    private bool isEnemy = false;
    private bool isDestructible = false;

    //Required components
    private Destructibles destructibleScript;

    //ANIMATION
    private Animator anim;
    [HideInInspector] public int isHitHash;

    private void Start() 
    {
        currentHealth = maxHealth;   
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
    }

    //quickly flashes gotHit for a frame then turns it off, for other scripts
    [HideInInspector] public bool gotHit;
    public void TakeDamage(int damageTaken)
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
        Debug.Log("Take damage");
        if(currentHealth <= 0)
        {
            if(isPlayer) PlayerDie();
            //else if (isEnemy) EnemyDie(); //note: handeled in the enemies respective state machine instead -lucas
            else if (isDestructible) DestroyDestructible();
        }
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
}

