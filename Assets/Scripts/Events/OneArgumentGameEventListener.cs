using UnityEngine;
using UnityEngine.Events;

public class OneArgumentGameEventListener<T> : MonoBehaviour
{
    public OneArgumentGameEvent<T> Event;
    public UnityEvent<T> Response;

    private void OnEnable()
    {
        Event.AddListener(this);
    }

    private void OnDisable()
    {
        Event.RemoveListener(this);        
    }

    public void OnEventRaised(T agr)
    {
        Response.Invoke(agr);
    }
}
