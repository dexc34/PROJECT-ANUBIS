using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoles : MonoBehaviour
{
    //Bullet holes dissapear after 5 seconds
    void Start()
    {
        Destroy(gameObject, 5);
    }   
}
