using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityActionTest : MonoBehaviour
{
    private UnityAction testing;
    private int index = 0;


    // Start is called before the first frame update
    void Start()
    {
        testing += TestAction;
        
    }

    private void Update() 
    {   
        if(Input.GetMouseButtonDown(0))
        {
            testing.Invoke();
        }

        
    }

    void TestAction()
    {
        Debug.Log("hi");
        index++;
        if(index< 5) return;
        if(testing.GetInvocationList().Length == 1)
        testing += OtherTest;
    }

    void OtherTest()
    {
        Debug.Log("test 2");
    }
}
