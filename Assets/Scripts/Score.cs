using Unity.Netcode;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] ulong scoreIndex;
    [SerializeField] int _score;

    [SerializeField] GameObject[] _stars;

    private void Start()
    {
        _score = 0;
    }

    public void InitScore()
    {
        _score = _stars.Length;

        foreach (var star in _stars)
        {
            star.SetActive(true);
        }
    }

    public void ResetScore()
    {
        _score = 0;

        foreach (var star in _stars)
        {
            star.SetActive(false);
        }
    }

    public void ChangeScore()
    {
        _stars[_score - 1].SetActive(false);
        _score--;

        if (_score > 0)
            return;

        ResetScore();
    }
}
