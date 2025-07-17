using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private float timeToCount = 1f;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnExperienceChange>(UpdateExperience);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnExperienceChange>(UpdateExperience);
    }

    private IEnumerator ExperienceRoutine(OnExperienceChange evt)
    {
        float elapsed = 0;
        float totalTime = timeToCount * Mathf.Abs(evt.delta);
        int experience = evt.previous;
        Debug.Log("Delta: " + evt.delta + ". Previous Experience: " + experience);
        while (elapsed < totalTime)
        {
            experience += evt.delta > 0 ? 1 : -1;
            experienceText.SetText(experience.ToString());
            elapsed += timeToCount;

            Debug.Log("Iterated exp: " + experience);
            yield return new WaitForSeconds(timeToCount);
        }
    }

    private void UpdateExperience(OnExperienceChange evt)
    {
        StopAllCoroutines();
        StartCoroutine(ExperienceRoutine(evt));
    }
}
