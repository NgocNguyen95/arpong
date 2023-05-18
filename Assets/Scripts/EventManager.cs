using UnityEngine.Events;

public class EventManager
{
    private static EventManager _instance;

    public UnityEvent eventBoardPlaced = new EventBoardPlaced();

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

public class EventBoardPlaced : UnityEvent { }
