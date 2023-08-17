using Unity.Netcode;
using UnityEngine;

public class Goal : NetworkBehaviour
{
    [SerializeField] ulong relayClientId;
    [SerializeField] UlongEvent GoalEvent;

    private bool _isOpen;

    public void OpenGoal(ulong clientId)
    {
        relayClientId = clientId;
        _isOpen = true;
    }

    public void CloseGoal()
    {
        relayClientId = ulong.MaxValue;
        _isOpen = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isOpen)
            return;

        if (other.gameObject.CompareTag("Ball"))
        {
            GoalClientRpc();
        }
    }

    [ClientRpc]
    private void GoalClientRpc()
    {
        GoalEvent.Raise(relayClientId);
    }
}
