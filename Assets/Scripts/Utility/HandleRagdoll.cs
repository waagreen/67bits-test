using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform anchorBone;

    [Header("Settings")]
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask groundedBlackList = 0, carriedBlackList = 0;

    private float followDelay, perlinSeed, swayAmplitude, swayFrequency;
    private Vector3 smoothedVelocity, smoothedAngularVelocity;
    private Vector3 localOffset;
    private Quaternion localRotation;
    private Rigidbody[] bodies;
    private Transform target = null;
    // Using constants because we are dealing with small numbers. Other values gives worse results.
    private const float kAmplitudeIncrease = 0.15f, kFrequencyIncrease = 0.1f, kStackPower = 1.2f;

    private void Awake()
    {
        bodies = GetComponentsInChildren<Rigidbody>();

        // This seed makes sure every body has a different motion
        perlinSeed = Random.Range(0f, 1000f);

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

        // Position and rotation relative to target
        Vector3 worldTargetPos = target.TransformPoint(localOffset);
        Quaternion worldTargetRot = target.rotation * localRotation;
        worldTargetPos += GetPerlinSway();

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

    private Vector3 GetPerlinSway()
    {
        // Perlin noise oscilation
        float time = Time.time * swayFrequency;
        float noiseX = (Mathf.PerlinNoise(perlinSeed, time) - 0.5f) * 2f;
        float noiseZ = (Mathf.PerlinNoise(perlinSeed + 100f, time) - 0.5f) * 2f;

        // Sway offset project on the XZ plane relative to target position
        Vector3 sway = new Vector3(noiseX, 0f, noiseZ) * swayAmplitude;
        return target.TransformDirection(sway);
    }

    public void SetRagdollKinematic(bool state)
    {
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = state;
        }
    }

    public void SetFollowDelay(float followDelay, int stackIndex, float swayFrequency, float swayAmplitude)
    {
        this.followDelay = Mathf.Max(0.01f, followDelay) * Mathf.Pow(stackIndex, kStackPower);
        this.swayAmplitude = swayAmplitude * (1f + (stackIndex * kAmplitudeIncrease));
        this.swayFrequency = swayFrequency * (1f + (stackIndex * kFrequencyIncrease));
    }


    public void AttachToTarget(Transform target, Vector3 localOffset, Quaternion localRotation)
    {
        this.target = target;
        this.localOffset = localOffset;
        this.localRotation = localRotation;

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

