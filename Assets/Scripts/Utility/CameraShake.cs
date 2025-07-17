using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    private void Awake()
    {
        originalPos = transform.localPosition;
        EventsManager.AddSubscriber<OnPunch>(Shake);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnPunch>(Shake);        
    }

    public void Shake(OnPunch evt)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(evt.duration, evt.magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
