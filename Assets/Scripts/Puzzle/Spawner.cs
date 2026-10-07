using System.Collections.Generic;
using UnityEngine;

public class Spawner
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

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

        if (Random.value < settings.helperChance && TrySpawnHelper(board))
        {
            return true;
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

    private bool TrySpawnHelper(Board board)
    {
        List<Vector2Int> itemCells = board.GetItemCells();

        for (int attempt = 0; attempt < 20 && itemCells.Count > 0; attempt++)
        {
            Vector2Int target = itemCells[Random.Range(0, itemCells.Count)];
            Vector2Int towardTap = Directions[Random.Range(0, Directions.Length)];

            List<Vector2Int> tapCells = GetEmptyRay(board, target, towardTap);
            if (tapCells.Count == 0)
            {
                continue;
            }

            Vector2Int tapCell = tapCells[Random.Range(0, tapCells.Count)];

            foreach (Vector2Int direction in Shuffled(Directions))
            {
                if (direction == -towardTap)
                {
                    continue;
                }

                Vector2Int helperCell = tapCell + direction;

                if (board.IsEmpty(helperCell))
                {
                    ItemType type = board.GetItem(target).GetItemType();
                    board.PlaceItem(new Item(type), helperCell);
                    return true;
                }
            }
        }

        return false;
    }

    private static List<Vector2Int> GetEmptyRay(Board board, Vector2Int from, Vector2Int direction)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        Vector2Int cell = from + direction;

        while (board.IsEmpty(cell))
        {
            cells.Add(cell);
            cell += direction;
        }

        return cells;
    }

    private static Vector2Int[] Shuffled(Vector2Int[] source)
    {
        Vector2Int[] result = (Vector2Int[])source.Clone();

        for (int i = result.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return result;
    }

    private static ItemType RandomType()
    {
        int typeCount = System.Enum.GetValues(typeof(ItemType)).Length;
        return (ItemType)Random.Range(0, typeCount);
    }
}
