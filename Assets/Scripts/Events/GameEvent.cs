using System.Collections.Generic;
using UnityEngine;

public class GameEvent<T> : ScriptableObject
{
    private List<GameEventListener<T>> listeners = new List<GameEventListener<T>>();

    public void Raise(T agr)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(agr);
        }
    }

    public void AddListener(GameEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener<T> listener)
    {
        listeners.Remove(listener);
    }
}
