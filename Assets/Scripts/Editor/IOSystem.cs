using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IOSystem : MonoBehaviour
{
    //Add or remove an InteractionEvent component to this game object
    public bool useEvents;
    public void BaseInput()
    {
        if (useEvents)
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact();
    }

    protected virtual void Interact()
    {
        //custom code is called through inherited scripts
    }
}
