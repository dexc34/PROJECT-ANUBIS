using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrage : MonoBehaviour
{
    //Editor tools
    [SerializeField]
    private bool stopAllFunctionality = false;


    [Header ("Stats")]

    [Tooltip ("How long it takes for the ability to become available after being used")]
    public float cooldown;

    [SerializeField]
    [Tooltip ("How big the grid of explosives will be (eg. 3 means 3x3 grid (9 explosives))")]
    private int gridSize;

    [SerializeField]
    [Tooltip ("How much space there will be bewteen each explosive")]
    private float explosiveSpacing;

    [SerializeField]
    [Tooltip ("How many units it'll take the explosives to reach their specified position")]
    private float zSpread;

    [SerializeField]
    [Tooltip ("What explosive will be thrown")]
    private GameObject barrageExplosivePrefab;


    [Header ("Audio")]

    [SerializeField]
    [Tooltip ("Sound that will play when using ability")]
    private AudioClip throwSFX;

    //Script variables
    private int totalGridSize;
    private Vector2[] explosivePostion;
    private bool isEven;

    //Required componets
    private Transform originPoint;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if(stopAllFunctionality) return;

        GetStats(GameObject.Find("Secondary Ability Manager").GetComponent<Barrage>());
        totalGridSize = gridSize * gridSize; 
        SetVectorPositions();
        audioSource = GetComponent<AudioSource>();
    }

    public void UseBarrage(Transform origin)
    {
        originPoint = origin;
        for(int i = 0; i < (totalGridSize); i++)
        {
            GameObject explosive = Instantiate(barrageExplosivePrefab, originPoint.position + originPoint.forward, originPoint.rotation);
            Vector3 bulletFinalDestination = (originPoint.right * explosivePostion[i].x) + (originPoint.up * explosivePostion[i].y) + (originPoint.forward * zSpread);
            explosive.GetComponent<Rigidbody>().AddForce(bulletFinalDestination * explosive.GetComponent<Explosion>().grenadeThrowForce/zSpread, ForceMode.Impulse);
        }

        audioSource.PlayOneShot(throwSFX);
    }

    private void SetVectorPositions()
    {
        explosivePostion = new Vector2[totalGridSize];
        if(totalGridSize % 2 == 0) isEven = true;
        else if(totalGridSize % 2 == 1) isEven = false;

        int gridPosition;
        Vector2 firstPosition;
        Vector2 currentPosition;

        if(isEven)
        {
            gridPosition = 0;
            firstPosition = new Vector2(0 - (explosiveSpacing/2) - (explosiveSpacing * (explosiveSpacing * (gridSize/2)) ), 0 + (explosiveSpacing/2) + (explosiveSpacing * (gridSize/2)));
            currentPosition = firstPosition;
        }
        else
        {
            gridPosition = 0;
            firstPosition = new Vector2(0 - explosiveSpacing - (explosiveSpacing * ((gridSize - 1) / 2)), 0 + explosiveSpacing + (explosiveSpacing * ((gridSize - 1) / 2) ));
            currentPosition = firstPosition;
        }
        
        for(int i = 0; i < totalGridSize; i ++)
        {
            explosivePostion[i] = new Vector2(currentPosition.x + explosiveSpacing, currentPosition.y);
            gridPosition ++;
            if(gridPosition == gridSize)
            {
                gridPosition = 0;
                currentPosition = (new Vector2(firstPosition.x, explosivePostion[i].y - explosiveSpacing));
            }
            else
            {
                currentPosition = explosivePostion[i];
            }
        }            
    }

    private void GetStats(Barrage barrageScriptToPullFrom)
    {
        //Stats
        cooldown = barrageScriptToPullFrom.cooldown;
        gridSize = barrageScriptToPullFrom.gridSize;
        explosiveSpacing = barrageScriptToPullFrom.explosiveSpacing;
        zSpread = barrageScriptToPullFrom.zSpread;
        barrageExplosivePrefab = barrageScriptToPullFrom.barrageExplosivePrefab;

        //Audio
        throwSFX = barrageScriptToPullFrom.throwSFX;
    }
}

