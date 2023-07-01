using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StaminBarValues : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject player;
    [SerializeField] Movement movementScript;
    [SerializeField] RectTransform barBounds;
    [SerializeField] Canvas canvas;

    [SerializeField] List<ProgressBar> staminaBars;

    [Header("Bar to Spawn")]
    [SerializeField] GameObject progressBarTemplate;

    [Header("Colors")]
    [SerializeField] Color barFullColor;
    [SerializeField] Color barRechargingColor;

    Vector3 sizeOfBounds;
    Vector3 startPosition;

    Vector3 spaceInBetweenBars = new Vector3(2,0,0);

    float totalStaminaBars;
    int currentStaminaBar;
    Vector2 calculatedSizeOfStaminaBar = new Vector2(0, 15);
    Vector3 widthOfEachBar;

    bool readyToUpdate = false;

    private void Awake()
    {
        movementScript = player.GetComponent<Movement>();
        totalStaminaBars = movementScript.maxStamina;
        currentStaminaBar = movementScript.maxStamina;

        //gets the starting position for where to spawn the bars
        startPosition = barBounds.transform.position;

        //gets the bounds
        sizeOfBounds.x = barBounds.rect.width;
        sizeOfBounds.y = barBounds.rect.height;

        //TEMP
        timeElapsed = 1.5f;

        CreateBars();
    }

    private void Update()
    {
        if(!movementScript.staminaCooldownDone)
            UpdateBars();
        if (movementScript.staminaCooldownDone)
            ResetTimer();
    }
    //creates the bar within the bounds of the reference in the canvas. can be scaled to any amount  
    void CreateBars()
    {
        calculatedSizeOfStaminaBar.x = (sizeOfBounds.x / totalStaminaBars) - (spaceInBetweenBars.x * (totalStaminaBars - 1) / totalStaminaBars);
        widthOfEachBar.x = calculatedSizeOfStaminaBar.x;
        for (int i = 0; i < totalStaminaBars; i++)
        {
            staminaBars.Add (Instantiate(progressBarTemplate, startPosition + (widthOfEachBar * i) + (spaceInBetweenBars * i), transform.localRotation).GetComponent<ProgressBar>());
            staminaBars[i].transform.SetParent(transform);
            staminaBars[i].color = barFullColor;
            //staminaBars[i].transform.localScale = new Vector3(1, 1, 1);
            RectTransform barWidth = staminaBars[i].GetComponent<RectTransform>();
            barWidth.sizeDelta = calculatedSizeOfStaminaBar;

            staminaBars[i].current = movementScript.staminaCooldown;
            staminaBars[i].maximum = movementScript.staminaCooldown;
        }
    }

    float timeElapsed;
    void UpdateBars()
    {
        if (movementScript.currentStamina > 0)
            currentStaminaBar = movementScript.currentStamina;
        else
            currentStaminaBar = 0;

        if (currentStaminaBar + 1 < totalStaminaBars)
        {
            staminaBars[currentStaminaBar + 1].current = 0;
        }

        if (!movementScript.staminaCooldownDone)
        {
            timeElapsed += Time.deltaTime;
        }
        staminaBars[currentStaminaBar].current = timeElapsed;
        if (timeElapsed >= 1.49f)
        {
            staminaBars[currentStaminaBar].color = barFullColor;
        }
        else
            staminaBars[currentStaminaBar].color = barRechargingColor;
    }

    void ResetTimer()
    {
        timeElapsed = 0;
    }
}
