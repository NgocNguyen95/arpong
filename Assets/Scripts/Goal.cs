using Unity.Netcode;
using UnityEngine;

public class Goal : NetworkBehaviour
{
    [SerializeField] ulong goalIndex;
    [SerializeField] UlongEvent GoalEvent;

    private bool _isOpen;

    private void Start()
    {
        _isOpen = false;
    }

    public void OpenGoal()
    {
        _isOpen = true;
    }

    public void CloseGoal()
    {
        _isOpen = false;
        gameObject.tag = "Untagged";
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
        GoalEvent.Raise(goalIndex);
    }
}
