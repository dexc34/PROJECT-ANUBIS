using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBarValues : MonoBehaviour
{
   [Header("References")]
   Health hpScript;
   [SerializeField] ProgressBar healthBar;
   //[SerializeField] Text healthValue;

    private void Start()
    {
        hpScript = GameObject.Find("Player").GetComponent<Health>();
    }
    private void Update()
    {
        healthBar.current = hpScript.currentHealth;
        healthBar.maximum = hpScript.maxHealth;
        //healthValue.text = hpScript.currentHealth.ToString();
    }
}
