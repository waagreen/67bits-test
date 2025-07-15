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

        foreach (var rb in bodies)
        {
            rb.MovePosition(rb.position + delta);
            rb.MoveRotation(deltaRot * rb.rotation);
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

        Vector3 worldTargetPos = target.TransformPoint(localOffset);
        Quaternion worldTargetRot = target.rotation * localRotation;

        Vector3 delta = worldTargetPos - anchorBone.position;
        Quaternion deltaRot = worldTargetRot * Quaternion.Inverse(anchorBone.rotation);

        foreach (var rb in bodies)
        {
            rb.excludeLayers = carriedBlackList;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            rb.MovePosition(rb.position + delta);
            rb.MoveRotation(deltaRot * rb.rotation);
            // rb.isKinematic = true;
        }
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

