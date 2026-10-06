using System.Collections.Generic;
using UnityEngine;

public class Line
{
    private List<Vector2Int> cells = new List<Vector2Int>();

    public void Start(Vector2Int cell)
    {
        cells.Clear();
        cells.Add(cell);
    }

    public bool TryExtend(Vector2Int cell)
    {
        if (!IsActive())
        {
            return false;
        }

        if (IsPreviousCell(cell))
        {
            cells.RemoveAt(cells.Count - 1);
            return true;
        }

        if (cells.Contains(cell) || !IsNeighbour(GetEnd(), cell))
        {
            return false;
        }

        cells.Add(cell);
        return true;
    }

    public bool IsPreviousCell(Vector2Int cell)
    {
        return cells.Count >= 2 && cells[cells.Count - 2] == cell;
    }

    public Vector2Int GetStart()
    {
        return cells[0];
    }

    public Vector2Int GetEnd()
    {
        return cells[cells.Count - 1];
    }

    public int GetLength()
    {
        return cells.Count;
    }

    public IReadOnlyList<Vector2Int> GetCells()
    {
        return cells;
    }

    public bool IsActive()
    {
        return cells.Count > 0;
    }

    public void Clear()
    {
        cells.Clear();
    }

    private static bool IsNeighbour(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }
}
