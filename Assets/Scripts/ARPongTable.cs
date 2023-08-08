using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPongTable : MonoBehaviour
{
    [Header("Goals")]
    [SerializeField] GameObject player1Goal;
    [SerializeField] GameObject player2Goal;
    [SerializeField] GameObject player3Goal;
    [SerializeField] GameObject player4Goal;

    [Header("Othes")]
    [SerializeField] GameObject cylinder;

    [Header("Prefabs")]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject paddlePrefab;


    private GameObject _ball;
    private GameObject _player1Paddle;
    private GameObject _player2Paddle;
    private GameObject _player3Paddle;
    private GameObject _player4Paddle;

    float _spawnRadius = 2f;
    float _paddleOffset = 0.5f;


    private void Start()
    {
        AddEventListeners();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }


    void AddEventListeners()
    {
        EventManager.Instance.boardPlacedEvent.AddListener(InitCloudAnchor);
        EventManager.Instance.goalEvent.AddListener(Goal);
    }


    void InitCloudAnchor()
    {
        var anchor = this.AddComponent<ARAnchor>();
        ARCloudAnchorManager.Instance.QueueAnchor(anchor);
    }


    void InitPlayGround()
    {
        SpawnBall();
        InitPaddle();
    }


    public void SpawnBall()
    {
        if (!NetworkManager.Singleton.IsServer)
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


    void InitPaddle()
    {
        Vector3 spawnPosition = player1Goal.transform.position + player1Goal.transform.TransformDirection(Vector3.up) * _paddleOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(player1Goal.transform.up);

        _player1Paddle = Instantiate(paddlePrefab, spawnPosition, spawnRotation);
    }


    void Goal(int playerNumber)
    {
        Destroy(_ball);

        Timer.StartCooldown(3f, 
            () => {
                SpawnBall();
            }
        );
    }


    void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(OnClientConnectedCallback)} {clientId}");

        if (!NetworkManager.Singleton.IsServer)
            return;

        if (clientId == 0)
        {
            Vector3 spawnPosition = player1Goal.transform.position + player1Goal.transform.TransformDirection(Vector3.up) * _paddleOffset;
            Quaternion spawnRotation = Quaternion.LookRotation(player1Goal.transform.up);

            _player1Paddle = Instantiate(paddlePrefab, spawnPosition, spawnRotation);
            _player1Paddle.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }
        else if (clientId == 1)
        {
            Vector3 spawnPosition = player2Goal.transform.position + player2Goal.transform.TransformDirection(Vector3.up) * _paddleOffset;
            Quaternion spawnRotation = Quaternion.LookRotation(player2Goal.transform.up);

            _player2Paddle = Instantiate(paddlePrefab, spawnPosition, spawnRotation);
            _player2Paddle.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }
    }

    void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"[{nameof(ARPongTable)}] {nameof(OnClientDisconnectCallback)} {clientId}");

        if (!NetworkManager.Singleton.IsServer)
            return;

        if (clientId == 0)
        {
            _player1Paddle.GetComponent<NetworkObject>().Despawn();
        }
        else if (clientId == 1)
        {
            _player2Paddle.GetComponent<NetworkObject>().Despawn();
        }
    }
}
