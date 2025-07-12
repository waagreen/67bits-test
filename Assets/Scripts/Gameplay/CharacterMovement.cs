using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField][Min(0f)] private float speed = 10f, acceleration = 30f, rotationSpeed = 50f;

    protected Rigidbody rb;
    protected Vector3 velocity = default;
    protected Vector3 desiredVelocity = default;
    protected Vector3 direction = default;

    public virtual Vector2 MovementInput { get; set; }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {      
        direction = new(MovementInput.x, 0f, MovementInput.y);
        desiredVelocity = speed * direction;
    }
    
    protected virtual void FixedUpdate()
    {
        HandleRotation();
        HandleVelocity();
    }

    private void HandleRotation()
    {
        if (direction.sqrMagnitude < 0.001f) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        Quaternion interpolatedRotation = Quaternion.Slerp
        (
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        rb.MoveRotation(interpolatedRotation);
    }

    private void HandleVelocity()
    {
        velocity = rb.linearVelocity;
        float t = acceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, t);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, t);

        rb.linearVelocity = velocity;
    }
}
