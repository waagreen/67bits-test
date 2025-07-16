using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Speed,
    Strength
}

public class SkillStore : MonoBehaviour
{
    // Key is the level, value is the "skill" (cost, received amount)
    private readonly Dictionary<int, Tuple<int, int>> speedTree = new();
    private readonly Dictionary<int, Tuple<int, int>> strengthTree = new();

    private const int kTreeLenght = 5; // How many skills aviable in each path

    // Cost doubles every level
    int GetCost(int level)
    {
        return (int)Mathf.Pow(2, level + 1);
    }

    // Value goes +1, +1, +2, +2, +3...
    int GetValue(int level)
    {
        return 1 + (level - 1) / 2;
    }

    public Tuple<int, int> GetSpeedSkill(int level)
    {
        if (speedTree.TryGetValue(level, out Tuple<int, int> skill)) return skill;
        else return null;
    }

    public Tuple<int, int> GetStrengthSkill(int level)
    {
        if (strengthTree.TryGetValue(level, out Tuple<int, int> skill)) return skill;
        else return null;
    }

    private void Start()
    {
        for (int i = 0; i < kTreeLenght; i++)
        {
            int value = GetValue(i);
            int cost = GetCost(i);

            speedTree[i] = new(cost, value);
            strengthTree[i] = new(cost, value);
        }
    }
}
