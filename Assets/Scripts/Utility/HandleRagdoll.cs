using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform anchorBone;

    [Header("Settings")]
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask groundedBlackList = 0, carriedBlackList = 0;

    private float followDelay;
    private Vector3 localOffset;
    private Rigidbody[] bodies;
    private Transform target = null;
    // Using constants because we are dealing with small numbers. Other values gives worse results.
    private const float kAmplitudeIncrease = 0.15f, kFrequencyIncrease = 0.1f, kStackPower = 1.2f;
    private Vector3 smoothedPosition, velocityRef;

    private void Awake()
    {
        bodies = GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < bodies.Length; i++)
        {
            Rigidbody body = bodies[i];
            body.isKinematic = !enableOnAwake;
            body.excludeLayers = groundedBlackList;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Position relative to target
        Vector3 worldTargetPos = target.TransformPoint(localOffset);

        // Calculating position with smooth damp to prevent movement overshoot
        smoothedPosition = Vector3.SmoothDamp
        (
            smoothedPosition,
            worldTargetPos,
            ref velocityRef,
            followDelay,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        // New velocity is the diference in position between target and body over physics step delta
        Vector3 linearVel = (smoothedPosition - anchorBone.position) / Time.fixedDeltaTime;

        foreach (var rb in bodies)
        {
            if (rb.isKinematic) continue; // skip if kinematic (safety check)
            rb.linearVelocity = linearVel;
        }
    }

    public void SetRagdollKinematic(bool state)
    {
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = state;
        }
    }

    public void SetFollowDelay(float followDelay, int stackIndex)
    {
        this.followDelay = Mathf.Max(0.01f, followDelay) * Mathf.Pow(stackIndex, kStackPower);
    }

    public void AttachToTarget(Transform target, Vector3 localOffset)
    {
        this.target = target;
        this.localOffset = localOffset;

        foreach (var rb in bodies) rb.excludeLayers = carriedBlackList;

        // Initializing position and velocity
        smoothedPosition = anchorBone.position;
        velocityRef = Vector3.zero;
    }

    public void AddImpulse(Vector3 force)
    {
        if (bodies == null) return;

        foreach (var body in bodies)
        {
            if (body.name.ToLower().Contains("spine"))
            {
                body.AddForceAtPosition(force, body.position, ForceMode.Impulse);
                break;
            }
        }
    }
}

