using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PaddleController : NetworkBehaviour
{
    public float speed = 10f;

    private Rigidbody rb;
    Touch touch;
    Vector3 targetPosition;
    private string goalTag = "Goal";

    private float offset = 0.5f;

    bool _stopMove;

    void Start()
    {
        // Get the rigidbody component attached to the paddle
        rb = GetComponent<Rigidbody>();
        EnhancedTouchSupport.Enable();

        targetPosition = rb.position;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (Touch.activeTouches.Count == 0)
            return;

        if (IsPointingOverUI())
            return;

        touch = Touch.activeTouches[0];
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == goalTag)
                {
                    var targetPoint = hit.point + hit.transform.TransformDirection(Vector3.up) * offset;
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
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);

        // Move the paddle to the new position
        rb.MovePosition(newPosition);
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
        targetPosition = targetPoint;
        _stopMove = false;
    }
}
