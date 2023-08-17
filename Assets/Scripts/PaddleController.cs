using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PaddleController : NetworkBehaviour
{
    [SerializeField] float speed = 10f;

    [Header("Events")]
    [SerializeField] UlongEvent playerJoin;
    [SerializeField] UlongEvent playerLeave;

    private Rigidbody _rb;
    Touch _touch;
    Vector3 _targetPosition;
    private string _goalTag = "Goal";

    private float _offset = 0.5f;

    bool _stopMove;

    void Start()
    {
        // Get the rigidbody component attached to the paddle
        _rb = GetComponent<Rigidbody>();
        EnhancedTouchSupport.Enable();

        _targetPosition = _rb.position;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (Touch.activeTouches.Count == 0)
            return;

        if (IsPointingOverUI())
            return;

        _touch = Touch.activeTouches[0];
        if (_touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(_touch.screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == _goalTag)
                {
                    var targetPoint = hit.point + hit.transform.TransformDirection(Vector3.up) * _offset;
                    MoveServerRPC(targetPoint);
                }
            }
        }
    }


    void FixedUpdate()
    {
        if (_stopMove)
            return;

        // Calculate the new position for the paddle based on the horizontal input
        Vector3 newPosition = Vector3.MoveTowards(_rb.position, _targetPosition, speed * Time.fixedDeltaTime);

        // Move the paddle to the new position
        _rb.MovePosition(newPosition);
    }


    bool IsPointingOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ball")
            return;

        _stopMove = true;
    }

    [ServerRpc]
    private void MoveServerRPC(Vector3 targetPoint)
    {
        Move(targetPoint);
    }

    private void Move(Vector3 targetPoint)
    {
        _targetPosition = targetPoint;
        _stopMove = false;
    }

    public override void OnNetworkSpawn()
    {
        playerJoin.Raise(OwnerClientId);

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        playerLeave.Raise(OwnerClientId);

        base.OnNetworkDespawn();
    }
}
