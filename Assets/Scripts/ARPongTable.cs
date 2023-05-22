using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPongTable : MonoBehaviour
{
    [SerializeField] GameObject player1Goal;
    [SerializeField] GameObject cylinder;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject paddlePrefab;


    private GameObject _ball;
    private GameObject _player1Paddle;

    float _spawnRadius = 2f;
    float _paddleOffset = 1.1f;


    private void Start()
    {
        AddEventListeners();
    }


    void AddEventListeners()
    {
        EventManager.Instance.eventBoardPlaced.AddListener(InitPlayGround);
        EventManager.Instance.eventGoal.AddListener(Goal);
    }


    void InitPlayGround()
    {
        InitBall();
        InitPaddle();
    }


    void InitBall()
    {
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
                InitBall();
            }
        );
    }
}
