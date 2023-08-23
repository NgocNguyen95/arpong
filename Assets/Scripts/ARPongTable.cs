using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPongTable : NetworkBehaviour
{
    public static ARPongTable Instance;

    [SerializeField] GameObject[] _goals;

    [Header("Othes")]
    [SerializeField] GameObject cylinder;

    [Header("Prefabs")]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject paddlePrefab;

    [Header("Events")]
    [SerializeField] UlongEvent playerJoin;


    private GameObject _ball;
    private Dictionary<int, GameObject> _paddles;

    float _spawnRadius = 2f;
    float _paddleOffset = 0.5f;
    const int _maxPlayers = 4;
    int _knockedOutPlayers = 0;

    bool _isGameOver;
    private string _goalTag = "Goal";
    private NetworkList<PlayerData> _playerDataNetworkList;


    private void Awake()
    {
        Instance = this;
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += OnPlayerDataNetworkListChanged;
    }

    private void Start()
    {
        _paddles = new Dictionary<int, GameObject>();
        AddEventListeners();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }


    void AddEventListeners()
    {
        EventManager.Instance.boardPlacedEvent.AddListener(InitCloudAnchor);
    }


    void InitCloudAnchor()
    {
        var anchor = this.AddComponent<ARAnchor>();
        ARCloudAnchorManager.Instance.QueueAnchor(anchor);
    }


    void InitPlayGround()
    {
        SpawnBall();
    }


    public void SpawnBall()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (_isGameOver)
            return;

        // Generate a random position within the spawn radius
        Vector3 randomOffset = Random.insideUnitSphere * _spawnRadius;
        Vector3 spawnPosition = cylinder.transform.position + randomOffset;

        // Offset the spawn position by the size of the target object's collider
        Collider targetCollider = cylinder.GetComponent<Collider>();
        if (targetCollider != null)
        {
            spawnPosition += targetCollider.bounds.extents;
        }

        // Instantiate the ball at the spawn position
        _ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        _ball.GetComponent<NetworkObject>().Spawn();
    }

    void CoolDownSpawnBall()
    {
        Destroy(_ball);

        Timer.StartCooldown(3f, 
            () => {
                SpawnBall();
            }
        );
    }

    void KnockOutPlayer(int playerIndex)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(KnockOutPlayer)} {playerIndex}");
        _knockedOutPlayers++;
        CheckWinner();
        if (!NetworkManager.Singleton.IsServer) return;

        if (_paddles[playerIndex] != null)
        {
            _paddles[playerIndex].GetComponent<NetworkObject>().Despawn();
            _paddles[playerIndex] = null;
        }
    }

    void GameOver()
    {
        _isGameOver = true;

        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].score > 0)
            {
                Debug.Log($"[{nameof(ARPongTable)}] {nameof(GameOver)} | winner: {i}");
                return;
            }
        }
    }

    void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(OnClientConnectedCallback)} {clientId}");
        _isGameOver = false;

        if (!NetworkManager.Singleton.IsServer)
            return;

        _playerDataNetworkList.Add(new PlayerData
        {
            relayClientId = clientId,
            score = 3
        });

        int slotIndex = _playerDataNetworkList.Count - 1;

        Vector3 spawnPosition = _goals[slotIndex].transform.position + _goals[slotIndex].transform.TransformDirection(Vector3.up) * _paddleOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(_goals[slotIndex].transform.up);

        var paddle = Instantiate(paddlePrefab, spawnPosition, spawnRotation);
        paddle.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        _paddles.Add(slotIndex, paddle);
    }

    void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(OnClientDisconnectCallback)} {clientId}");

        if (!NetworkManager.Singleton.IsServer)
            return;

        int playerIndex = GetPlayerIndexByClientId(clientId);

        if (playerIndex == -1 || _paddles[playerIndex] == null)
            return;

        _paddles[playerIndex].GetComponent<NetworkObject>().Despawn();
        _paddles[playerIndex] = null;

        if (_isGameOver)
            return;

        var playerData = _playerDataNetworkList[playerIndex];
        playerData.score = 0;
        _playerDataNetworkList[playerIndex] = playerData;
    }

    public int GetPlayerIndexByClientId(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].relayClientId == clientId)
                return i;
        }

        return -1;
    }

    void OnPlayerDataNetworkListChanged(NetworkListEvent<PlayerData> networkListEvent)
    {
        var eventType = networkListEvent.Type;

        switch (eventType)
        {
            case NetworkListEvent<PlayerData>.EventType.Add:
                if (NetworkManager.Singleton.LocalClientId == networkListEvent.Value.relayClientId)
                {
                    _goals[networkListEvent.Index].tag = _goalTag;
                }

                playerJoin.Raise((ulong)networkListEvent.Index);
                break;
            case NetworkListEvent<PlayerData>.EventType.Value:
                if (networkListEvent.Value.score > 0)
                    return;

                KnockOutPlayer(networkListEvent.Index);                
                break;
        }
    }

    public void OnGoalEvent(ulong goalIndex)
    {
        CoolDownSpawnBall();
        UpdateScore((int)goalIndex);
    }

    void UpdateScore(int goalIndex)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        var playerData = _playerDataNetworkList[goalIndex];
        playerData.score--;
        _playerDataNetworkList[goalIndex] = playerData;
    }

    void CheckWinner()
    {
        if (_knockedOutPlayers < _playerDataNetworkList.Count - 1)
            return;

        GameOver();
    }
}
