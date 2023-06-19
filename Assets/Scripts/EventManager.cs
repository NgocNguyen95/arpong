using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
    private static EventManager _instance;

    public BoardPlacedEvent boardPlacedEvent = new BoardPlacedEvent();
    public CloudAnchorResolvedEvent cloudAnchorResolvedEvent = new CloudAnchorResolvedEvent();
    public GoalEvent goalEvent = new GoalEvent();

    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EventManager();
            return _instance;
        }
    }
}

public class BoardPlacedEvent : UnityEvent { }

public class CloudAnchorResolvedEvent : UnityEvent<Transform> { }

public class GoalEvent : UnityEvent<int> { }
