using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    [Tooltip ("Amount of health object starts with")]
    private int maxHealth = 100;

    //Script variables
    private int currentHealth;
    private bool isPlayer = false;
    private bool isEnemy = false;
    private bool isDestructible = false;
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
        currentHealth -= damageTaken;
        if(currentHealth <= 0)
        {
            if(isPlayer) PlayerDie();
            else if (isEnemy) EnemyDie();
            else if (isDestructible) DestroyDestructible();
        }
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
