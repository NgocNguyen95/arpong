using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f;

    private Rigidbody rb;
    private float horizontalInput;
    Touch touch;
    Vector3 targetPosition;

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
        if (Touch.activeTouches.Count == 0)
        {
            return;
        }

        if (IsPointingOverUI())
            return;

        touch = Touch.activeTouches[0];

        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(ray.origin, hit.point, Color.yellow);
                Debug.Log($"[{nameof(PaddleController)}] {nameof(Update)} hit: {hit.collider.name}");

                if (hit.collider.tag == "Wall")
                {
                    Debug.Log($"[{nameof(PaddleController)}] {nameof(Update)} hit point: {hit.point}");
                    var targetPoint = hit.point + hit.transform.TransformDirection(Vector3.forward) * 1.5f;
                    Debug.Log($"[{nameof(PaddleController)}] {nameof(Update)} targetPoint: {targetPoint}");
                    targetPosition = targetPoint;
                    _stopMove = false;
                }
            }
        }
    }


    void FixedUpdate()
    {
        if (_stopMove)
            return;

        Debug.Log($"[{nameof(PaddleController)}] {nameof(FixedUpdate)} hitPoint: {targetPosition}");

        // Calculate the new position for the paddle based on the horizontal input
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);

        // Move the paddle to the new position
        rb.MovePosition(newPosition);
    }


    Vector3 HitTest()
    {       
        Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(ray.origin, hit.point, Color.yellow);
            Debug.Log($"[{nameof(PaddleController)}] {nameof(HitTest)} hit: {hit.collider.name}");

            if (hit.collider.tag == "Wall")
            {
                Debug.Log($"[{nameof(PaddleController)}] {nameof(HitTest)} hit point: {hit.point}");
                var targetPoint = hit.point + hit.transform.TransformDirection(Vector3.forward) * 1.5f;
                return hit.point;
            }
        }
        return Vector3.zero;        
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
}
