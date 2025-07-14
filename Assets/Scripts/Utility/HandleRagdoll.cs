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
}
