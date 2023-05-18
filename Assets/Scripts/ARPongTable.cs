using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPongTable : MonoBehaviour
{
    [SerializeField] GameObject goalPlayer1;
    [SerializeField] GameObject cylinder;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject paddlePrefab;


    private GameObject _ball;
    private GameObject _paddlePlayer1;

    float _spawnRadius = 2f;
    float _paddleOffset = 1.5f;


    private void Start()
    {
        AddEventListeners();
    }


    void AddEventListeners()
    {
        EventManager.Instance.eventBoardPlaced.AddListener(InitPlayGround);
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
        Vector3 spawnPosition = goalPlayer1.transform.position + goalPlayer1.transform.TransformDirection(Vector3.up) * _paddleOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(goalPlayer1.transform.up);

        _paddlePlayer1 = Instantiate(paddlePrefab, spawnPosition, spawnRotation);
    }
}
