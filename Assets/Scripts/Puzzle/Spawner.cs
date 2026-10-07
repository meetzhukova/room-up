using UnityEngine;

public class Spawner
{
    private float interval;
    private float minInterval;
    private float speedUp;
    private float lowFillThreshold;
    private float lowFillMultiplier;
    private float timer;

    public Spawner(float startInterval, float minInterval, float speedUp,
        float lowFillThreshold, float lowFillMultiplier)
    {
        interval = startInterval;
        this.minInterval = minInterval;
        this.speedUp = speedUp;
        this.lowFillThreshold = lowFillThreshold;
        this.lowFillMultiplier = lowFillMultiplier;
    }

    public bool Tick(float deltaTime, float fillRatio)
    {
        timer += deltaTime;

        if (timer < GetCurrentInterval(fillRatio))
        {
            return false;
        }

        timer = 0f;
        interval = Mathf.Max(minInterval, interval * speedUp);
        return true;
    }

    public float GetCurrentInterval(float fillRatio)
    {
        if (fillRatio >= lowFillThreshold)
        {
            return interval;
        }

        float t = fillRatio / lowFillThreshold;
        return interval * Mathf.Lerp(lowFillMultiplier, 1f, t);
    }

    public bool SpawnItem(Board board)
    {
        var emptyCells = board.GetEmptyCells();

        if (emptyCells.Count == 0)
        {
            return false;
        }

        int typeCount = System.Enum.GetValues(typeof(ItemType)).Length;
        Vector2Int cell = emptyCells[Random.Range(0, emptyCells.Count)];
        ItemType type = (ItemType)Random.Range(0, typeCount);

        board.PlaceItem(new Item(type), cell);
        return true;
    }
}
