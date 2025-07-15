using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image experienceFill;
    [SerializeField] private float timeToFill = 1f, timeToCount = 1f;

    private void Start()
    {
        EventsManager.AddSubscriber<OnXpGain>(UpdateLevel);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnXpGain>(UpdateLevel);
    }

    private IEnumerator LevelRoutine(int experience)
    {
        float elapsed = 0f;
        int xpDelta = 0;
        while (elapsed < timeToCount)
        {
            levelText.SetText(Math.Max(xpDelta++, experience).ToString());

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void UpdateLevel(OnXpGain evt)
    {
        StopAllCoroutines();
        StartCoroutine(LevelRoutine(evt.amount));
    }
}
