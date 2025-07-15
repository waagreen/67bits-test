using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform anchorBone;

    [Header("Settings")]
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask groundedBlackList = 0, carriedBlackList = 0;

    [System.Serializable]
    private struct Bone
    {
        public Transform transform;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    private bool detachOnReachDestination = false, canBeCollected = false;
    private float followDelay;
    private Vector3 smoothedPosition, velocityRef;
    private Vector3 localOffset;
    private Transform target = null;
    private Rigidbody[] bodies;
    private Bone[] initalPose;

    public bool CanBeCollected => canBeCollected;

    private void Awake()
    {
        bodies = GetComponentsInChildren<Rigidbody>();
        initalPose = new Bone[bodies.Length];

        for (int i = 0; i < bodies.Length; i++)
        {
            Rigidbody body = bodies[i];
            Transform t = body.transform;

            initalPose[i] = new()
            {
                transform = t,
                localPosition = t.localPosition,
                localRotation = t.localRotation
            };

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

        TryToDetach(worldTargetPos);
    }

    private void TryToDetach(Vector3 worldTargetPos)
    {
        if (detachOnReachDestination && Vector3.Distance(anchorBone.position, worldTargetPos) <= 0.05f)
        {
            target = null;
            detachOnReachDestination = false;

            foreach (var body in bodies)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.excludeLayers = groundedBlackList;
            }
            EventsManager.Broadcast(new OnDropCorpse { id = transform.parent.GetInstanceID()});
        }
    }

    public void RestoreInitialState()
    {
        // Restore skeleton to the inital position;
        for (int i = 0; i < bodies.Length; i++)
        {
            Rigidbody body = bodies[i];
            Bone bone = initalPose[i];

            body.excludeLayers = groundedBlackList;
            bone.transform.SetLocalPositionAndRotation(bone.localPosition, bone.localRotation);
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
        }

        // Make sure state variables are also cleaned
        smoothedPosition = anchorBone.position;
        velocityRef = Vector3.zero;
        canBeCollected = false;
        detachOnReachDestination = false;
        target = null;
    }

    public void DisableKinematics()
    {
        canBeCollected = true;
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = false;
        }
    }

    public void SetFollowDelay(float followDelay, int stackIndex)
    {
        this.followDelay = Mathf.Max(0.01f, followDelay) * stackIndex;
    }

    public void AttachToTarget(Transform target, Vector3 localOffset, bool detachOnReachDestination = false)
    {
        this.target = target;
        this.localOffset = localOffset;
        this.detachOnReachDestination = detachOnReachDestination;

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

