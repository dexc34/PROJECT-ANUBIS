using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverArea : MonoBehaviour
{

    private Cover[] covers;

    void Awake()
    {
        covers = GetComponentsInChildren<Cover>();
    }

    public Cover GetRandomCover(Vector3 agentLocation) //gets a random cover, but we can utilize agentLocation to be more specific
    {
        return covers[Random.Range(0, covers.Length - 1)];
    }

  
}
