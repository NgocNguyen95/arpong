using UnityEngine.Events;

public class EventManager
{
    private static EventManager _instance;

    public BoardPlacedEvent boardPlacedEvent = new BoardPlacedEvent();
    public JoinRoomEvent joinRoomEvent = new JoinRoomEvent();
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

public class JoinRoomEvent : UnityEvent { }

public class GoalEvent : UnityEvent<int> { }
