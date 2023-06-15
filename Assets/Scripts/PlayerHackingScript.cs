using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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

    [Header(
        "How wide the outline \n" +
        "will be when the \n" +
        "player looks at them\n")]
    [SerializeField]private float highlightWidth = 7;

    private GameObject currentlySelectedEnemy = null;
    private Outline enemiesOutline = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Raycasting();
    }

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
                    Destroy(currentlySelectedEnemy.GetComponent<Outline>());
                    enemiesOutline = null;
                    currentlySelectedEnemy = null;
                }
            }


            //runs if the object hit with the raycast has the Hackable tag
            if (hit.collider.gameObject.CompareTag("Hackable"))
            {
                //stores the enemy's game object and also adds an outline to it
                currentlySelectedEnemy = hit.collider.gameObject;
                enemiesOutline = currentlySelectedEnemy.gameObject.AddComponent<Outline>();
                
                //turns on the outline of the selected enemy, sets it's colour and it's width
                enemiesOutline.enabled = true;
                enemiesOutline.OutlineColor = highlightColour;
                enemiesOutline.OutlineWidth = highlightWidth;

                //some debug stuff we can get rid of later
                Debug.Log("yep");
                Debug.DrawRay(raycastStart, raycastDirection * hit.distance, Color.yellow);
            }
        }
        else
        {
            //deletes the enemy's outline and unstores the gameobject and outline
            if (currentlySelectedEnemy != null)
            {
                Destroy(currentlySelectedEnemy.GetComponent<Outline>());
                enemiesOutline = null;
                currentlySelectedEnemy = null;
            }

            Debug.DrawRay(raycastStart, raycastDirection * 1000, Color.white);
            Debug.Log("nope");
        }
    }
}
