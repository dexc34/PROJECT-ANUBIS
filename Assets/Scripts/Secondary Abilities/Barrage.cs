using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Barrage : MonoBehaviour
{
    //Script variables
    [HideInInspector] public float cooldown = 12;
    private float explosiveSpacing = 1;
    private int gridSize = 3;
    private int totalGridSize;
    private Vector2[] explosivePostion;
    private bool isEven;
    private float zSpread = 5;

    //Required componets
    private GameObject barrageExplosivePrefab;
    private Transform virtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponentInChildren<CameraMove>().gameObject.transform;
        barrageExplosivePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Barrage Explosive.prefab", typeof(GameObject));
        totalGridSize = gridSize * gridSize; 
        SetVectorPositions();
    }

    public void UseBarrage()
    {
        Debug.Log("Do barrage");
        for(int i = 0; i < (totalGridSize); i++)
        {
            GameObject explosive = Instantiate(barrageExplosivePrefab, virtualCamera.position + virtualCamera.forward, virtualCamera.rotation);
            Vector3 bulletFinalDestination = (virtualCamera.right * explosivePostion[i].x) + (virtualCamera.up * explosivePostion[i].y) + (virtualCamera.forward * zSpread);
            explosive.GetComponent<Rigidbody>().AddForce(bulletFinalDestination * explosive.GetComponent<Explosion>().grenadeThrowForce/zSpread, ForceMode.Impulse);
        }
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
            Debug.Log(firstPosition);
            currentPosition = firstPosition;
        }
        else
        {
            gridPosition = 0;
            firstPosition = new Vector2(0 - explosiveSpacing - (explosiveSpacing * ((gridSize - 1) / 2)), 0 + explosiveSpacing + (explosiveSpacing * ((gridSize - 1) / 2) ));
            Debug.Log(firstPosition);
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
            Debug.Log(explosivePostion[i]);
        }            
    }
}

