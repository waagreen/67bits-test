using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField][Min(0f)] protected float speed = 10f, acceleration = 30f, rotationSpeed = 50f;
    [SerializeField][Range(0f, 1f)] private float lookSmoothTime = 0.1f;

    protected CharacterAnimator anim;
    protected Rigidbody rb;
    protected Vector3 velocity = default, desiredVelocity = default, lookVelocity = default;
    protected Vector3 direction = default, smoothLookDirection = default;

    public virtual Vector2 MovementInput { get; set; }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<CharacterAnimator>();
        smoothLookDirection = transform.forward;
    }

    protected virtual void Update()
    {
        direction = new(MovementInput.x, 0f, MovementInput.y);
        desiredVelocity = speed * direction;

        if (anim) anim.Movement = direction;
    }
    
    protected virtual void FixedUpdate()
    {
        HandleRotation();
        HandleVelocity();
    }

    private void HandleRotation()
    {
        if (direction.sqrMagnitude < 0.01f) return;

        smoothLookDirection = Vector3.SmoothDamp
        (
            smoothLookDirection,
            direction,
            ref lookVelocity,
            lookSmoothTime
        );

        float angle = Mathf.Atan2(smoothLookDirection.x, smoothLookDirection.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
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
