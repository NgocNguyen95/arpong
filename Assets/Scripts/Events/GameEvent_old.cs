using System.Collections.Generic;
using UnityEngine;

public class GameEvent_old<T> : ScriptableObject
{
    private List<GameEventListener_old<T>> listeners = new List<GameEventListener_old<T>>();

    public void Raise(T arg)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(arg);
        }
    }

    public void AddListener(GameEventListener_old<T> listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener_old<T> listener)
    {
        listeners.Remove(listener);
    }
}
