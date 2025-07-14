using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask collisionBlacklist = 0;
    private Rigidbody[] bodies;

    private void Start()
    {
        bodies = GetComponentsInChildren<Rigidbody>();
        SetState(enableOnAwake);
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

    public void SetState(bool state)
    {
        if (bodies == null) return;

        // If the body is kinematic, then we are giving control over to the physics engine
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = !state;
            body.excludeLayers = collisionBlacklist;
        }
    }

    public Transform GetRootForCarrying()
    {
        if (bodies == null) return transform;

        foreach (var body in bodies)
        {
            body.isKinematic = false;
            body.useGravity = false;
        }

        // Skeleton parent
        return transform;
    }
}
