using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private Image levelFill;
    [SerializeField] private float timeToFill = 1f, timeToCount = 1f;

    private void Start()
    {
        EventsManager.AddSubscriber<OnXpGain>(UpdateExperience);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnXpGain>(UpdateExperience);
    }

    private IEnumerator ExperienceRoutine(int experience)
    {
        int xpDelta = 1;
        float elapsed = 0;
        float time = timeToCount * experience;

        while (elapsed < time)
        {
            experienceText.SetText(Math.Min(xpDelta++, experience).ToString());
            elapsed += timeToCount;

            yield return new WaitForSeconds(timeToCount);
        }
    }

    private void UpdateExperience(OnXpGain evt)
    {
        StopAllCoroutines();
        StartCoroutine(ExperienceRoutine(evt.amount));
    }
}
