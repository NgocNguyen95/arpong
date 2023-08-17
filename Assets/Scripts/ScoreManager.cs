using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public UlongEvent knockedOutEvent;
    public GameEvent gameOverEvent;

    [SerializeField] GameObject[] _scores;
    [SerializeField] GameObject[] _goals;

    private Dictionary<ulong, PlayerData> players;

    // Start is called before the first frame update
    void Start()
    {
        players = new Dictionary<ulong, PlayerData>();
    }

    public void AddNewPlayer(ulong clientId)
    {
        if (!players.ContainsKey(clientId))
        {
            var newPlayerData = ScriptableObject.CreateInstance<PlayerData>();
            newPlayerData.relayClientId = clientId;
            newPlayerData.score = 3;
            players.Add(clientId, newPlayerData);
            _scores[players.Count - 1].GetComponent<Score>().InitScore(clientId);
            _goals[players.Count - 1].GetComponent<Goal>().OpenGoal(clientId);
        }
    }

    public void RemovePlayer(ulong clientId)
    {
        if (players.ContainsKey(clientId))
        {
            Destroy(players[clientId]);
            players.Remove(clientId);
            _scores[players.Count].GetComponent<Score>().ResetScore();
            _goals[players.Count].GetComponent<Goal>().CloseGoal();
        }
    }

    [ClientRpc]
    public void UpdateScoresClientRpc(ulong clientId)
    {
        if (!players.ContainsKey(clientId))
            return;

        players[clientId].score--;
        Debug.Log($"[{nameof(ScoreManager)}] {nameof(UpdateScoresClientRpc)} player {clientId} score: {players[clientId].score}");

        if (players[clientId].score > 0)
            return;

        Debug.Log($"[{nameof(ScoreManager)}] {nameof(UpdateScoresClientRpc)} player {clientId} knocked out");
        knockedOutEvent.Raise(clientId);
        RemovePlayer(clientId);

        CheckWinner();
    }

    void CheckWinner()
    {
        if (players.Count > 1)
            return;

        gameOverEvent.Raise();

        foreach(var key in players.Keys)
        {
            Debug.Log($"[{nameof(ScoreManager)}] {nameof(CheckWinner)} | player {key} win");
        }
    }
}
