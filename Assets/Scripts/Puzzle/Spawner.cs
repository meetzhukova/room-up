using System.Collections.Generic;
using UnityEngine;

public class Spawner
{
    private SpawnSettings settings;
    private float interval;
    private float timer;

    public Spawner(SpawnSettings settings)
    {
        this.settings = settings;
        interval = settings.startInterval;
    }

    public bool Tick(float deltaTime, float fillRatio)
    {
        timer += deltaTime;

        if (timer < GetCurrentInterval(fillRatio))
        {
            return false;
        }

        timer = 0f;
        interval = Mathf.Max(settings.minInterval, interval * settings.speedUp);
        return true;
    }

    public float GetCurrentInterval(float fillRatio)
    {
        if (fillRatio >= settings.lowFillThreshold)
        {
            return interval;
        }

        float t = fillRatio / settings.lowFillThreshold;
        return interval * Mathf.Lerp(settings.lowFillMultiplier, 1f, t);
    }

    public int SpawnBatch(Board board)
    {
        int count = 1;

        while (count < settings.maxItemsPerSpawn && Random.value < settings.extraItemChance)
        {
            count++;
        }

        int spawned = 0;

        for (int i = 0; i < count; i++)
        {
            if (!SpawnItem(board))
            {
                break;
            }

            spawned++;
        }

        return spawned;
    }

    public bool SpawnItem(Board board)
    {
        if (!board.HasEmptyCell())
        {
            return false;
        }

        SpawnRandom(board);
        return true;
    }

    private void SpawnRandom(Board board)
    {
        List<Vector2Int> emptyCells = board.GetEmptyCells();
        Vector2Int cell = emptyCells[Random.Range(0, emptyCells.Count)];
        board.PlaceItem(new Item(RandomType()), cell);
    }

    private static ItemType RandomType()
    {
        int typeCount = System.Enum.GetValues(typeof(ItemType)).Length;
        return (ItemType)Random.Range(0, typeCount);
    }
}
