using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPongTable : MonoBehaviour
{
    [SerializeField] GameObject[] _goals;

    [Header("Othes")]
    [SerializeField] GameObject cylinder;

    [Header("Prefabs")]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject paddlePrefab;


    private GameObject _ball;
    private GameObject[] _paddles = new GameObject[4];

    float _spawnRadius = 2f;
    float _paddleOffset = 0.5f;

    bool _isGameOver;
    private string _goalTag = "Goal";


    private void Start()
    {
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

    public void CoolDownSpawnBall(ulong unused)
    {
        Destroy(_ball);

        Timer.StartCooldown(3f, 
            () => {
                SpawnBall();
            }
        );
    }

    public void KnockOutPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        _paddles[clientId].GetComponent<NetworkObject>().Despawn();
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(OnClientConnectedCallback)} {clientId}");

        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            _goals[clientId].tag = _goalTag;
        }

        if (!NetworkManager.Singleton.IsServer)
            return;

        _isGameOver = false;

        Vector3 spawnPosition = _goals[clientId].transform.position + _goals[clientId].transform.TransformDirection(Vector3.up) * _paddleOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(_goals[clientId].transform.up);

        _paddles[clientId] = Instantiate(paddlePrefab, spawnPosition, spawnRotation);
        _paddles[clientId].GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }

    void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(OnClientDisconnectCallback)} {clientId}");

        if (!NetworkManager.Singleton.IsServer)
            return;

        if (_paddles[clientId] != null)
            _paddles[clientId].GetComponent<NetworkObject>().Despawn();
    }
}
