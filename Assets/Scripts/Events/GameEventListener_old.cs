using UnityEngine;
using UnityEngine.Events;

public class GameEventListener_old<T> : MonoBehaviour
{
    public GameEvent_old<T> Event;
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
