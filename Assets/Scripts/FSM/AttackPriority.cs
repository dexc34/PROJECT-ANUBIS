using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script used to communicate between enemy state machines and the attack coordinator 
public class AttackPriority : MonoBehaviour
{
    //does this enemy have a token?
    [HideInInspector] public bool hasToken;

    //has this script already been used when checking for tokens?
    [HideInInspector] public bool hasBeenUsed = false;


    [HideInInspector] public float attackPriority;

    [HideInInspector] public AttackCoordinator ac;

    //int idNumber;
    void Start()
    {
        ac = GameObject.Find("Attack Coordinator").GetComponent<AttackCoordinator>();
        AddThisToBackLog();
    }

    public void AddThisToBackLog()
    {
        ac.AddToBackLog(this);
        //idNumber = ac.idNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
