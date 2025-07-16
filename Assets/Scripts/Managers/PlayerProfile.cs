using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 6f;
    [SerializeField] private int defaultStrenght = 3, defaultExperience = 0;

    private float currentSpeed;
    private int currentStrength;
    private static int currentExperience;
    public static int Experience => currentExperience;

    private void Start()
    {
        currentExperience = defaultExperience;
        currentStrength = defaultStrenght;
        currentSpeed = defaultSpeed;

        EventsManager.Broadcast(new OnLevelUp
        {
            speed = currentSpeed,
            strength = currentStrength
        });
        EventsManager.AddSubscriber<OnExperienceChange>(UpdateExperience);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnExperienceChange>(UpdateExperience);
    }

    private void UpdateExperience(OnExperienceChange evt)
    {
        currentExperience += evt.delta;
    }
}
