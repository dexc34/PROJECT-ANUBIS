using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    [SerializeField] BaseState _initialState;
    //caches componets so we dont need to keep instantiating them for each action
    private Dictionary<Type, Component> _cachedComponents;

    private void Awake()
    {
        CurrentState = _initialState;
    }

    //gets and sets the current state (other scripts calls this function to put its action in it)
    public BaseState CurrentState
    {
        get;
        set;
    }

    //executes the current state 
    private void Update()
    {
        {
            CurrentState.Execute(this);
        }
    }

    public new T GetComponent<T>() where T : Component
    {
        if (_cachedComponents.ContainsKey(typeof(T)))
            return _cachedComponents[typeof(T)] as T;

        var component = base.GetComponent<T>();
        if (component != null)
        {
            _cachedComponents.Add(typeof(T), component);
        }
        return component;
    }
}
