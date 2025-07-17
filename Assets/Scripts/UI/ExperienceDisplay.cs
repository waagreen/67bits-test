using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private float timeToCount = 1f;
    [SerializeField] private AudioSource source;
    [SerializeField] private List<AudioClip> clips;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnExperienceChange>(UpdateExperience);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnExperienceChange>(UpdateExperience);
    }

    private void PlayAudioFeedback(int delta)
    {
        if (source == null || clips == null || clips.Count < 1) return;
        source.clip = clips[delta < 1 ? 0 : 1];
        source.Play();
    }

    private IEnumerator ExperienceRoutine(OnExperienceChange evt)
    {
        float elapsed = 0;
        float totalTime = timeToCount * Mathf.Abs(evt.delta);
        int experience = evt.previous;
        while (elapsed < totalTime)
        {
            experience += evt.delta > 0 ? 1 : -1;
            experienceText.SetText(experience.ToString());
            elapsed += timeToCount;
            PlayAudioFeedback(evt.delta);
            yield return new WaitForSeconds(timeToCount);
        }
    }

    private void UpdateExperience(OnExperienceChange evt)
    {
        StopAllCoroutines();
        StartCoroutine(ExperienceRoutine(evt));
    }
}
