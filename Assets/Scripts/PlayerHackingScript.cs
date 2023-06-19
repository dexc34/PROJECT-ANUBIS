using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHackingScript : MonoBehaviour
{
    private float raycastDistance = Mathf.Infinity;

    [Header(
        "The colour enemies \n" +
        "will be highlighted \n" +
        "with when the player \n" +
        "looks at them\n"
        )]
    [SerializeField]private Color highlightColour = Color.red;
    [SerializeField]private Color hackingColour = Color.green;

    [Header(
        "How wide the outline \n" +
        "will be when the \n" +
        "player looks at them\n")]
    [SerializeField]private float highlightWidth = 7;

    [SerializeField] private InputActionReference hackButton;
    [SerializeField] [Tooltip("Player Camera prefab goes here")] private GameObject cameraPrefab;
    private Cinemachine.CinemachineBrain mainCameraBrain;
    private GameObject newCamera;

    private GameObject currentlySelectedEnemy = null;
    private Outline selectedEnemiesOutline = null;

    private GameObject currentlyHackingEnemy = null;
    private Outline hackingEnemiesOutline = null;

    private GameObject currentlyStoredEnemy = null;

    private CharacterController characterController;

    [SerializeField] private LayerMask ignoredLayer;

    private float hackingTimer = -1000;

    private bool hacking = false;




    //Built in stuff
    //##############################################################################################
    void Start()
    {
        characterController= GetComponent<CharacterController>();
        mainCameraBrain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Cinemachine.CinemachineBrain>();
    }


    void Update()
    {
        if (hacking && hackButton.ToInputAction().WasReleasedThisFrame())
            ExitHackMode();

        if (!hacking)
            Raycasting();

        RunTimer();
    }






    //The Raycasting
    //##############################################################################################
    public void Raycasting()
    {
        //stores the data of the object that has been hit by the raycast
        RaycastHit hit;

        //the start and direction of the raycast (the location of the camera and which direction its pointing)
        Vector3 raycastStart = Camera.main.transform.position;
        Vector3 raycastDirection = Camera.main.transform.TransformDirection(Vector3.forward);



        //runs the code inside if a raycast from raycastStart to raycastDistance pointed in the direction of racastDirection hits anything with a collider
        if (Physics.Raycast(raycastStart, raycastDirection, out hit, raycastDistance))
        {
            //checks to see if the object currently in the raycast is the enemy that is currently being stored
            if (hit.collider.gameObject != currentlySelectedEnemy)
            {
                //deletes the enemy's outline and unstores the gameobject and outline
                if (currentlySelectedEnemy != null)
                {
                    if (currentlySelectedEnemy != currentlyHackingEnemy)
                        Destroy(currentlySelectedEnemy.GetComponent<Outline>());

                    selectedEnemiesOutline = null;
                    currentlySelectedEnemy = null;
                }
            }


            //runs if the object hit with the raycast has the Hackable tag
            if (hit.collider.gameObject.CompareTag("Hackable"))
            {
                if (currentlySelectedEnemy == null)
                {
                    //stores the enemy's game object and also adds an outline to it
                    currentlySelectedEnemy = hit.collider.gameObject;
                    selectedEnemiesOutline = currentlySelectedEnemy.gameObject.AddComponent<Outline>();
                
                    //turns on the outline of the selected enemy, sets it's colour and it's width
                    selectedEnemiesOutline.enabled = true;
                    selectedEnemiesOutline.OutlineColor = highlightColour;
                    selectedEnemiesOutline.OutlineWidth = highlightWidth;
                }

                //some debug stuff we can get rid of later
                Debug.DrawRay(raycastStart, raycastDirection * hit.distance, Color.yellow);

                if (!hacking && hackButton.ToInputAction().WasPressedThisFrame())
                {
                    currentlyHackingEnemy = currentlySelectedEnemy;
                    hackingEnemiesOutline = currentlyHackingEnemy.GetComponent<Outline>();
                    hackingEnemiesOutline.OutlineColor = hackingColour;

                    StartTimer(1);
                    hacking= true;
                }
            }
        }
        else
        {
            //deletes the enemy's outline and unstores the gameobject and outline
            if (currentlySelectedEnemy != null)
            {
                if (currentlySelectedEnemy != currentlyHackingEnemy)
                    Destroy(currentlySelectedEnemy.GetComponent<Outline>());

                selectedEnemiesOutline = null;
                currentlySelectedEnemy = null;
            }

            Debug.DrawRay(raycastStart, raycastDirection * 1000, Color.white);
        }
    }





    //Timer Stuff
    //##############################################################################################
    public void StartTimer(float timerDurration)
    {
        hackingTimer = timerDurration;
    }


    private void RunTimer()
    {
        if (hackingTimer >0)
        {
            hackingTimer -= Time.deltaTime;
        }
        else if (hackingTimer < 0.1f && hackingTimer > -0.1f)
        {
            HackEnemy();
        }
    }




    //Hacking Stuff
    //##############################################################################################
    private void HackEnemy()
    {
        Debug.Log("hacked");

        //releases the currently stored enemy if there is one
        if (currentlyStoredEnemy != null)
        {
            //sets the enemy back to being it's own parent and turns it back on
            currentlyStoredEnemy.transform.parent = null;
            currentlyStoredEnemy.SetActive(true);
        }

        //stores the currently hacking enemy as a variable
        currentlyStoredEnemy = currentlyHackingEnemy;

        //teleports to the position of the currently stored
        //enemy and facing in the same direction (the character
        //controller has to be turned off because it messes up
        //the teleport)
        characterController.enabled= false;
        StartCoroutine("CameraTransition");
        transform.position = currentlyStoredEnemy.transform.position;
        transform.rotation = currentlyStoredEnemy.transform.rotation;

        //Parents new camera to the player
        newCamera.transform.parent = transform;
        

        //turns off the currently stored enemy and makes it the child of the player to be released later
        currentlyStoredEnemy.SetActive(false);
        currentlyStoredEnemy.transform.parent = transform;

        ExitHackMode();
    }


    public void ExitHackMode()
    {
        hacking = false;

        //resets the timer to a very low number so that it doesn't do anything (see RunTimer())
        hackingTimer = -1000;

        //gets rid of the outline of the enemy and then sets the
        //variables that store the game object and the outline to
        //nothing
        Destroy(currentlyHackingEnemy.GetComponent<Outline>());
        currentlyHackingEnemy = null;
        hackingEnemiesOutline= null;
    }

    private IEnumerator CameraTransition()
    {
        //Creates a new camera inside a set point of the hacked enemy
        GameObject currentCamera  = transform.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().gameObject;
        newCamera = Instantiate(cameraPrefab, currentlyStoredEnemy.transform.Find("Camera Spawn Point").transform.position, currentlyStoredEnemy.transform.Find("Camera Spawn Point").transform.rotation);

        //Unparents both cameras to avoid affecting their positions accidentally
        currentCamera.transform.parent = null;
        newCamera.transform.parent = null;

        //Adds 1 to the priority parameter so that it will automatically target the new camera
        newCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority + 1;
         
        //Waits until the the transition (or camera blend) is over to continue the code, this parameter should be dynamic in the future
        yield return new WaitForSeconds(mainCameraBrain.m_DefaultBlend.BlendTime);

        //Returns control to the player, destroys old camera
        characterController.enabled = true;
        Destroy(currentCamera);
    }
}