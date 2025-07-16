using System.Collections.Generic;
using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
    [SerializeField] private List<Color> colors;
    [SerializeField] private Renderer targetRenderer;

    private void SwapColor(OnLevelUp evt)
    {
        int colorIndex = (PlayerProfile.StrengthLevel + PlayerProfile.SpeedLevel) % colors.Count;
        targetRenderer.material.color = colors[colorIndex];
    }

    private void Start()
    {
        EventsManager.AddSubscriber<OnLevelUp>(SwapColor);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnLevelUp>(SwapColor);
    }
}
