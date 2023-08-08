using System.Collections.Generic;
using UnityEngine;

public class OneArgumentGameEvent<T> : ScriptableObject
{
    private List<OneArgumentGameEventListener<T>> listeners = new List<OneArgumentGameEventListener<T>>();

    public void Raise(T arg)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(arg);
        }
    }

    public void AddListener(OneArgumentGameEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(OneArgumentGameEventListener<T> listener)
    {
        listeners.Remove(listener);
    }
}
