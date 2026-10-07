using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    [Header("Timer")]
    public float startInterval = 3f;
    public float minInterval = 1f;
    [Range(0.5f, 1f)] public float speedUp = 0.97f;

    [Header("Low fill boost")]
    [Range(0f, 1f)] public float lowFillThreshold = 0.3f;
    [Range(0.1f, 1f)] public float lowFillMultiplier = 0.25f;

    [Header("Several items at once")]
    [Range(0f, 1f)] public float extraItemChance = 0.3f;
    [Range(1, 5)] public int maxItemsPerSpawn = 3;

    [Header("Cleared cells")]
    public float clearedCellCooldown = 1.5f;

    [Header("Helper spawn")]
    [Range(0f, 1f)] public float helperChance = 0.3f;
}
