using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillStoreUI : MonoBehaviour
{
    [SerializeField] private SkillStore storeController;
    [SerializeField] private RectTransform storeContent;
    [SerializeField] private Button closeButton;
    [SerializeField] private SkillButton speedButton, strengthButton;

    private void Start()
    {
        closeButton.onClick.AddListener(CloseStore);
        EventsManager.AddSubscriber<OnStoreInteraction>(OpenStore);

        RefreshStore();
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(CloseStore);
        EventsManager.RemoveSubscriber<OnStoreInteraction>(OpenStore);
    }

    private void RefreshButton(SkillButton btn, Tuple<int, int> skill, int experience, SkillType type)
    {
        if (skill != null)
        {
            btn.Refresh(skill, type);
            btn.State = experience >= skill.Item1;
        }
        else btn.State = false;
    }

    private void RefreshStore()
    {
        // Update buttons state based on current level skill cost

        int experience = PlayerProfile.Experience;
        Tuple<int, int> speedSkill = storeController.GetSpeedSkill(PlayerProfile.SpeedLevel);
        Tuple<int, int> strengthSkill = storeController.GetStrengthSkill(PlayerProfile.StrengthLevel);

        RefreshButton(speedButton, speedSkill, experience, SkillType.Speed);
        RefreshButton(strengthButton, strengthSkill, experience, SkillType.Strength);
    }

    private void CloseStore()
    {
        storeContent.gameObject.SetActive(false);
    }

    private void OpenStore(OnStoreInteraction evt)
    {
        storeContent.gameObject.SetActive(true);   
        RefreshStore();
    }
}
