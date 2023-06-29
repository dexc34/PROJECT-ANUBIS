using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarValues : MonoBehaviour
{
    [Header("References")]
    SecondaryAbility abilityScript;
    [SerializeField] ProgressBar abilityBar;
    [SerializeField] GameObject barVisuals;

    [Header("Colors")]
    [SerializeField] Color barFullColor;
    [SerializeField] Color barRechargingColor;

    float timeElapsed;
    private void Start()
    {
        abilityScript = GameObject.Find("Player").GetComponent<SecondaryAbility>();
        abilityBar.maximum = abilityScript.abilityCooldown;
        abilityBar.current = abilityScript.abilityCooldown;
    }

    private void Update()
    {
        if (!abilityScript.canUseAbility)
            UpdateBar();
        if (abilityScript.canUseAbility)
            ResetTimer();
    }

    void UpdateBar()
    {
        abilityBar.color = barRechargingColor;
        abilityBar.maximum = abilityScript.abilityCooldown;
        timeElapsed += Time.deltaTime;
        abilityBar.current = timeElapsed;
    }
    void ResetTimer()
    {
        abilityBar.color = barFullColor;
        timeElapsed = 0;
    }
}
