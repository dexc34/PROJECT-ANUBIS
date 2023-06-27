using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminBarValues : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject player;
    [SerializeField] PlayerMovement movementScript;
    [SerializeField] List<ProgressBar> staminaBars;
    [SerializeField] RectTransform barBounds;
    [SerializeField] Canvas canvas;

    [Header("Bar to Spawn")]
    [SerializeField] GameObject progressBarTemplate;

    float staminaCooldownMax;

    Vector3 sizeOfBounds;
    Vector3 startPosition;

    Vector3 spaceInBetweenBars = new Vector3(15,0,0);
    Vector3 tempWidth = new Vector3(200, 0, 0);

    float totalStaminaBars;
    float currentStaminaBar;
    Vector2 calculatedSizeOfStaminaBar = new Vector2(0, 15);
    Vector3 widthOfEachBar;

    private void Awake()
    {
        movementScript = player.GetComponent<PlayerMovement>();
        totalStaminaBars = movementScript.amountOfDashes;
        currentStaminaBar = movementScript.currentDashes;

        //gets the starting position for where to spawn the bars
        startPosition = barBounds.transform.position;

        //gets the bounds
        sizeOfBounds.x = barBounds.rect.width;
        sizeOfBounds.y = barBounds.rect.height;

        Debug.Log("Size of bounds: " + sizeOfBounds);
        Debug.Log("Starting Position for spawn: " + startPosition);

        CreateBars();
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
            RectTransform barWidth = staminaBars[i].GetComponent<RectTransform>();
            barWidth.sizeDelta = calculatedSizeOfStaminaBar;

            staminaBars[i].maximum = movementScript.dashCooldown;
        }
    }

    void UpdateBars()
    {
        if (currentStaminaBar < totalStaminaBars)
            currentStaminaBar = movementScript.currentDashes + 1;
        else
            currentStaminaBar = movementScript.currentDashes;

    }
}
