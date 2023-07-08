using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IOSystem : MonoBehaviour
{
    public void BaseInput()
    {
        Output();
    }

    protected virtual void Output()
    {

    }
}
