using Unity.Netcode;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] ulong _clientId;
    [SerializeField] int _score;

    [SerializeField] GameObject[] _stars;

    public void InitScore(ulong clientId)
    {
        _clientId = clientId;
        _score = _stars.Length;

        foreach (var star in _stars)
        {
            star.SetActive(true);
        }
    }

    public void ResetScore()
    {
        _clientId = ulong.MaxValue;
        _score = 0;

        foreach (var star in _stars)
        {
            star.SetActive(false);
        }
    }

    public void ChangeScore(ulong clientId)
    {
        if (clientId != _clientId)
            return;

        if (_score == 0)
            return;

        _stars[_score - 1].SetActive(false);
        _score--;
    }
}
