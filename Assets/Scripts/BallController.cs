using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float boostSpeed = 10f;

    private string connerTag = "Corner";
    private Rigidbody rb;

    void Start()
    {
        // Get the rigidbody component attached to the ball
        rb = GetComponent<Rigidbody>();

        // Generate a random initial velocity for the ball
        Vector3 initialVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * baseSpeed;
        rb.velocity = initialVelocity;
    }

    void FixedUpdate()
    {
        // Move the ball
        rb.MovePosition(transform.position + rb.velocity * Time.fixedDeltaTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(connerTag))
        {
            rb.velocity = rb.velocity.normalized * boostSpeed;
        }
        else
        {
            rb.velocity = rb.velocity.normalized * baseSpeed;
        }
    }
}
