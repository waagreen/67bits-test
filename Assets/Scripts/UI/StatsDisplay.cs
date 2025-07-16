using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text strengthDisplay, speedDisplay;

    private void UpdateDisplay(OnLevelUp evt)
    {
        speedDisplay.SetText($"Speed {evt.speed}");
        strengthDisplay.SetText($"Strength {evt.strength}");
    }

    private void Start()
    {
        EventsManager.AddSubscriber<OnLevelUp>(UpdateDisplay);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnLevelUp>(UpdateDisplay);
    }
}
