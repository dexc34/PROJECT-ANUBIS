using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenHacking : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player").GetComponent<PlayerHackingScript>().transitioningBetweenEnemies && transform.parent.name == "Weapon Holder")
        {
            Destroy(gameObject);
        }
    }
}
