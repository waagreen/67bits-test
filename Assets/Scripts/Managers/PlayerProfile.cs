using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 6f;
    [SerializeField] private int defaultStrenght = 3, defaultExperience = 0;

    private float currentSpeed;
    private int currentStrength;
    private static int currentExperience, speedLevel, strengthLevel;

    public static int Experience => currentExperience;
    public static int SpeedLevel => speedLevel;
    public static int StrengthLevel => strengthLevel;

    private void Start()
    {
        currentExperience = defaultExperience;
        currentStrength = defaultStrenght;
        currentSpeed = defaultSpeed;

        speedLevel = strengthLevel = 0;

        EventsManager.Broadcast(new OnLevelUp
        {
            speed = currentSpeed,
            strength = currentStrength
        });

        EventsManager.AddSubscriber<OnExperienceChange>(UpdateExperience);
        EventsManager.AddSubscriber<OnSkillUpgrade>(UpdateSkillLevel);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnExperienceChange>(UpdateExperience);
        EventsManager.RemoveSubscriber<OnSkillUpgrade>(UpdateSkillLevel);
    }

    private void UpdateExperience(OnExperienceChange evt)
    {
        currentExperience += evt.delta;
    }

    private void UpdateSkillLevel(OnSkillUpgrade evt)
    {
        switch (evt.type)
        {
            case SkillType.Speed:
                speedLevel++;
                currentSpeed += evt.value;
                break;
            case SkillType.Strength:
                strengthLevel++;
                currentStrength += evt.value;
                break;
        }

        EventsManager.Broadcast(new OnLevelUp { strength = currentStrength, speed = currentSpeed });   
        EventsManager.Broadcast(new OnExperienceChange { previous = currentExperience, delta = -evt.cost });
        currentExperience = Mathf.Max(0, currentExperience - evt.cost);
    }
}
