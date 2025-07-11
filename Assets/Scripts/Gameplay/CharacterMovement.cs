using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField][Min(0f)] private float speed = 10f, acceleration = 30f, rotationSpeed = 50f;

    // Variables set on start --
    private Rigidbody rb;
    private InputActions inputs;

    // Variables updated at runtime
    private Vector3 velocity;
    private Vector3 desiredVelocity;
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        inputs = new();
        inputs.Enable();
    }

    private void Update()
    {
        Vector2 movementInput = inputs.Player.Move.ReadValue<Vector2>();
        
        direction = new(movementInput.x, 0f, movementInput.y);
        desiredVelocity = speed * direction;
    }

    private void HandleRotation()
    {
        if (direction.sqrMagnitude < 0.001f) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        Quaternion interpolatedRotation = Quaternion.Slerp(
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

    private void FixedUpdate()
    {
        HandleRotation();
        HandleVelocity();
    }

    private void OnDestroy()
    {
        inputs.Disable();
        inputs = null;
    }
}
