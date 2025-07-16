using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text amount, cost;

    private Tuple<int, int> skill;
    private SkillType? type;

    public bool State
    {
        get => btn.interactable;
        set
        {
            btn.interactable = value;
        }
    }

    private void Start()
    {
        btn.onClick.AddListener(UpgradeSkill);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(UpgradeSkill);
    }

    private void UpgradeSkill()
    {
        if ((skill == null) || (type == null)) return;

        EventsManager.Broadcast(new OnSkillUpgrade { type = type.Value, cost = skill.Item1, value = skill.Item2 });
        EventsManager.Broadcast(new OnStoreInteraction());
    }

    public void Refresh(Tuple<int, int> skill, SkillType type)
    {
        this.skill = skill;
        this.type = type;

        amount.SetText($"Recieve +{skill.Item2}");
        cost.SetText($"Pay {skill.Item1} experience");
    }
}
