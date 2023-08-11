using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] ulong relayClientId;
    public UlongEvent GoalEvent;

    public void SetRelayClientId(ulong clientId)
    {
        relayClientId = clientId;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            GoalEvent.Raise(relayClientId);
        }
    }
}
