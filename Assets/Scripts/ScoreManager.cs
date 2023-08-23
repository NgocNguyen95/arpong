using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] GameObject[] _scores;
    [SerializeField] GameObject[] _goals;

    public void AddNewPlayer(ulong playerIndex)
    {
        for (int i = (int)playerIndex; i >= 0; i--)
        {
            _scores[i].GetComponent<Score>().InitScore();
            _goals[i].GetComponent<Goal>().OpenGoal();
        }
    }

    public void RemovePlayer(ulong clientId)
    {
        int playerIndex = ARPongTable.Instance.GetPlayerIndexByClientId(clientId);

        if (playerIndex == -1)
            return;

        _scores[playerIndex].GetComponent<Score>().ResetScore();
        _goals[playerIndex].GetComponent<Goal>().CloseGoal();
    }

    public void UpdateScores(ulong goalIndex)
    {
        Debug.Log($"[{nameof(ScoreManager)}] {nameof(UpdateScores)} goal {goalIndex}");

        _scores[(int)goalIndex].GetComponent<Score>().ChangeScore();
    }
}
