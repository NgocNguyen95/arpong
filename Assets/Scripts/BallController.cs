using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody rb;

    void Start()
    {
        // Get the rigidbody component attached to the ball
        rb = GetComponent<Rigidbody>();

        // Generate a random initial velocity for the ball
        Vector3 initialVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        rb.velocity = initialVelocity;
    }

    void FixedUpdate()
    {
        // Move the ball
        rb.MovePosition(transform.position + rb.velocity * Time.fixedDeltaTime);
    }
}
