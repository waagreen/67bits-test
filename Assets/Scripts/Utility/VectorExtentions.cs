using UnityEngine;

public static class VectorExtentions
{
    public const float kEpsilon = 0.0001f;

    public static bool HasMeaningfulValue(this Vector3 vec)
    {
        return vec.sqrMagnitude >= kEpsilon;
    }
}
