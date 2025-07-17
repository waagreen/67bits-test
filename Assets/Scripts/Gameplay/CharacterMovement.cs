using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField][Min(0f)] protected float speed = 10f, acceleration = 30f, rotationSpeed = 50f;
    [SerializeField][Range(0f, 1f)] private float lookSmoothTime = 0.1f;
    [SerializeField] private RandomSoundPlayer stepSounds;

    protected Rigidbody rb;
    protected CharacterAnimator anim;
    protected Vector3 velocity, desiredVelocity, lookVelocity, direction, smoothLookDirection;
    protected Vector2 movementInput;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<CharacterAnimator>();
        smoothLookDirection = transform.forward;
    }

    protected virtual void Update()
    {
        direction = new(movementInput.x, 0f, movementInput.y);

        bool hasValue = direction.HasMeaningfulValue();

        desiredVelocity = hasValue ? speed * direction : Vector3.zero;
        if (anim) anim.Movement = hasValue ? direction : Vector2.zero;
    }

    protected virtual void FixedUpdate()
    {
        if (direction.HasMeaningfulValue())
        {
            HandleRotation();
            HandleVelocity();
            if (stepSounds != null) stepSounds.Play(); 
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void HandleRotation()
    {
        smoothLookDirection = Vector3.SmoothDamp(smoothLookDirection, direction, ref lookVelocity, lookSmoothTime);

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
