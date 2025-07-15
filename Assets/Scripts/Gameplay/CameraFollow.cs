using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 cameraOffset = new(0, 5, -7);
    [SerializeField][Min(0f)] private float lookSpeed = 5f, distance = 1f, collectDistanceDelta = 0.5f, distanceDeltaDuration = 1f;
    [SerializeField][Range(0f, 1f)] private float rotationLag = 0.1f;

    private Vector3 currentVelocity;
    private float originalDistance;

    private void Start()
    {
        originalDistance = distance;
        EventsManager.AddSubscriber<OnCollect>(UpdateDistance);
        EventsManager.AddSubscriber<OnXpGain>(ResetDistance);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCollect>(UpdateDistance);
        EventsManager.RemoveSubscriber<OnXpGain>(ResetDistance);
    }

    private IEnumerator DistanceCoroutine(float newDistance)
    {
        float elapsed = 0f;

        while (elapsed < distanceDeltaDuration)
        {
            float delta = Time.deltaTime;
            distance = Mathf.Lerp(distance, newDistance, delta);
            elapsed += delta;
            yield return null;
        }
    }

    private void ResetDistance(OnXpGain evt)
    {
        StopAllCoroutines();
        StartCoroutine(DistanceCoroutine(originalDistance));
    }

    private void UpdateDistance(OnCollect evt)
    {
        StopAllCoroutines();
        float amount = distance + collectDistanceDelta * evt.amount;
        StartCoroutine(DistanceCoroutine(amount));
    }

    private void LateUpdate()
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
