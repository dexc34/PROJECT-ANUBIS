using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarValues : MonoBehaviour
{
    [Header("References")]
    SecondaryAbility abilityScript;
    [SerializeField] ProgressBar abilityBar;

    private void Start()
    {
        abilityScript = GameObject.Find("Player").GetComponent<SecondaryAbility>();
    }
}
