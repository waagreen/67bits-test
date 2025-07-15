using System.Collections;
using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform anchorBone;

    [Header("Settings")]
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask groundedBlackList = 0, carriedBlackList = 0;

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

        Vector3 velocity = delta / Time.fixedDeltaTime;

        // Convert delta rotation to angular velocity
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 angularVelocity = angle * Mathf.Deg2Rad * axis.normalized / Time.fixedDeltaTime;
        angularVelocity = (axis == Vector3.zero) ? Vector3.zero : angularVelocity;

        foreach (var rb in bodies)
        {
            if (rb.isKinematic) continue; // skip if kinematic (safety check)

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, velocity, 0.5f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, angularVelocity, 0.5f);
        }
    }

    public void SetRagdollKinematic(bool state)
    {
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = state;
        }
    }

    public void AttachToTarget(Transform target, Vector3 desiredLocalOffset, Quaternion desiredLocalRotation)
    {
        this.target = target;
        localOffset = desiredLocalOffset;
        localRotation = desiredLocalRotation;

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

