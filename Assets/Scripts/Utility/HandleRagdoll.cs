using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform anchorBone;

    [Header("Settings")]
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask groundedBlackList = 0, carriedBlackList = 0;

    private float followDelay = 0f;
    private Vector3 smoothedVelocity, smoothedAngularVelocity;
    private Vector3 localOffset;
    private Quaternion localRotation;
    private Rigidbody[] bodies;
    private Transform target = null;


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

        Vector3 worldTargetPos = target.TransformPoint(localOffset);
        Quaternion worldTargetRot = target.rotation * localRotation;

        Vector3 delta = worldTargetPos - anchorBone.position;
        Quaternion deltaRot = worldTargetRot * Quaternion.Inverse(anchorBone.rotation);

        Vector3 targetVelocity = delta / Time.fixedDeltaTime;

        // Convert delta rotation to angular velocity
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 targetAngularVelocity = angle * Mathf.Deg2Rad * axis.normalized / Time.fixedDeltaTime;
        targetAngularVelocity = (axis == Vector3.zero) ? Vector3.zero : targetAngularVelocity;

        // Introducing "inertia" by adding delay while updating upper bodies position and rotation
        float lerpFactor = Mathf.Clamp01(Time.fixedDeltaTime / followDelay);
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, targetVelocity, lerpFactor);
        smoothedAngularVelocity = Vector3.Lerp(smoothedAngularVelocity, targetAngularVelocity, lerpFactor);

        foreach (var rb in bodies)
        {
            if (rb.isKinematic) continue; // skip if kinematic (safety check)

            rb.linearVelocity = smoothedVelocity;
            rb.angularVelocity = smoothedAngularVelocity;
        }
    }


    public void SetRagdollKinematic(bool state)
    {
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = state;
        }
    }

    public void AttachToTarget(Transform target, Vector3 localOffset, Quaternion localRotation, float followDelay)
    {
        this.target = target;
        this.localOffset = localOffset;
        this.localRotation = localRotation;
        this.followDelay = Mathf.Max(0.01f, followDelay);

        foreach (var rb in bodies) rb.excludeLayers = carriedBlackList;     
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

