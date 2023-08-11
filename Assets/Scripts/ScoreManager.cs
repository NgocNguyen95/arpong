using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<ulong, PlayerData> players;

    public UlongEvent knockedOutEvent;
    public GameEvent gameOverEvent;

    // Start is called before the first frame update
    void Start()
    {
        players = new Dictionary<ulong, PlayerData>();
        NetworkManager.Singleton.OnClientConnectedCallback += AddNewPlayer;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddNewPlayer(ulong clientId)
    {
        if (!players.ContainsKey(clientId))
        {
            var newPlayerData = ScriptableObject.CreateInstance<PlayerData>();
            newPlayerData.relayClientId = clientId;
            newPlayerData.score = 3;
            players.Add(clientId, newPlayerData);
        }
    }

    void RemovePlayer(ulong clientId)
    {
        if (players.ContainsKey(clientId))
        {
            Destroy(players[clientId]);
            players.Remove(clientId);
        }
    }

    public void Goal(ulong clientId)
    {
        if (!players.ContainsKey(clientId))
            return;

        players[clientId].score--;
        Debug.Log($"[{nameof(ScoreManager)}] {nameof(Goal)} player {clientId} score: {players[clientId].score}");

        if (players[clientId].score > 0)
            return;

        Debug.Log($"[{nameof(ScoreManager)}] {nameof(Goal)} player {clientId} knocked out");
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
