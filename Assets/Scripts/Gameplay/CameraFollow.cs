using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 cameraOffset = new(0, 5, -7);
    [SerializeField][Min(0f)] private float lookSpeed = 5f, distance = 1f;
    [SerializeField][Range(0f, 1f)] private float rotationLag = 0.1f;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        // Offset always oriented on world axis
        Vector3 basePosition = target.position + cameraOffset;
        Vector3 desiredPosition = basePosition - transform.forward * distance;

        // Smooth camera positioning
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, rotationLag);

        // Smooth look rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
    }
}
