using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StaminBarValues : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject player;
    [SerializeField] PlayerMovement movementScript;
    [SerializeField] RectTransform barBounds;
    [SerializeField] Canvas canvas;

    [SerializeField] List<ProgressBar> staminaBars;

    [Header("Bar to Spawn")]
    [SerializeField] GameObject progressBarTemplate;

    [Header("Colors")]
    [SerializeField] Color barFullColor;
    [SerializeField] Color barRechargingColor;

    float staminaCooldownMax;

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
        movementScript = player.GetComponent<PlayerMovement>();
        totalStaminaBars = movementScript.amountOfDashes;
        currentStaminaBar = movementScript.amountOfDashes;

        //gets the starting position for where to spawn the bars
        startPosition = barBounds.transform.position;

        //gets the bounds
        sizeOfBounds.x = barBounds.rect.width;
        sizeOfBounds.y = barBounds.rect.height;

        Debug.Log("Size of bounds: " + sizeOfBounds);
        Debug.Log("Starting Position for spawn: " + startPosition);

        //TEMP
        timeElapsed = 1.5f;

        CreateBars();
    }

    private void Update()
    {
        if(!movementScript.dashCooldownDone)
            UpdateBars();
        if (movementScript.dashCooldownDone)
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

            staminaBars[i].current = movementScript.dashCooldown;
            staminaBars[i].maximum = movementScript.dashCooldown;
        }
    }

    float timeElapsed;
    void UpdateBars()
    {
        if (movementScript.currentDashes > 0)
            currentStaminaBar = movementScript.currentDashes;
        else
            currentStaminaBar = 0;

        if (currentStaminaBar + 1 < totalStaminaBars)
        {
            Debug.Log("Set bar in front to zero");
            staminaBars[currentStaminaBar + 1].current = 0;
        }

        if (!movementScript.dashCooldownDone)
        {
            Debug.Log("add to time");
            timeElapsed += Time.deltaTime;
        }
        staminaBars[currentStaminaBar].current = timeElapsed;
        if (timeElapsed >= 1.49f)
        {
            staminaBars[currentStaminaBar].color = barFullColor;
        }
        else
            staminaBars[currentStaminaBar].color = barRechargingColor;
        Debug.Log(currentStaminaBar);
    }

    void ResetTimer()
    {
        Debug.Log("Used dash");
        timeElapsed = 0;
    }
}
