using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenHacking : MonoBehaviour
{
    void Update()
    {
        //if the player is hacking and the parent of the object is the player's weapon holder
        if (GameObject.Find("Player").GetComponent<PlayerHackingScript>().transitioningBetweenEnemies && transform.parent.name == "Weapon Holder")
        {
            Destroy(gameObject);
        }
    }
}
