using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminBarValues : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject player;
    [SerializeField] PlayerMovement movementScript;
    [SerializeField] ProgressBar[] staminaBarsScript;
    [SerializeField] RectTransform barBounds;
    [SerializeField] Canvas canvas;

    GameObject[] staminaBars;

    [Header("Bar to Spawn")]
    [SerializeField] GameObject progressBarTemplate;

    float staminaCooldownMax;

    Vector3 sizeOfBounds;
    Vector3 startPosition;

    Vector3 spaceInBetweenBars = new Vector3(15,0,0);
    Vector3 tempWidth = new Vector3(200, 0, 0);

    float totalStaminaBars;
    float currentStaminaBar;
    float calculatedSizeOfStaminaBar;
    Vector3 widthOfEachBar;

    private void Awake()
    {
        //movementScript = player.GetComponent<PlayerMovement>();
        //totalStaminaBars = movementScript.amountOfDashes;
        //currentStaminaBar = movementScript.currentDashes;

        //TEMP
        totalStaminaBars = 2;

        startPosition = barBounds.transform.position;

        sizeOfBounds.x = barBounds.rect.width;
        sizeOfBounds.y = barBounds.rect.height;

        Debug.Log("Size of bounds: " + sizeOfBounds);
        Debug.Log("Starting Position for spawn: " + startPosition);

        CreateBars();
    }

    void CreateBars()
    {
        //Debug.Log("Create bars");
        //Instantiate(progressBarTemplate, startPosition + spaceInBetweenBars, transform.localRotation).transform.parent = transform;
        //Instantiate(progressBarTemplate, startPosition + tempWidth + spaceInBetweenBars, transform.localRotation).transform.parent = transform;
        calculatedSizeOfStaminaBar = sizeOfBounds.x / totalStaminaBars;
        widthOfEachBar.x = calculatedSizeOfStaminaBar;
        for (int i = 0; i <= totalStaminaBars; i++)
        {
            if (i != 0)
            {
                staminaBars[i] = Instantiate(progressBarTemplate, startPosition, transform.localRotation);
            }
            else
                staminaBars[i] = Instantiate(progressBarTemplate, startPosition + widthOfEachBar, transform.localRotation);
            RectTransform barWidth = staminaBars[i].GetComponent<RectTransform>();
            staminaBars[i].transform.parent = transform;
        }
    }

}
