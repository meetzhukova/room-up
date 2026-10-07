using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private int width;
    private int height;
    private Item[,] cells;

    public Board(int width, int height)
    {
        this.width = width;
        this.height = height;
        cells = new Item[width, height];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width
            && cell.y >= 0 && cell.y < height;
    }

    public Item GetItem(Vector2Int cell)
    {
        if (!IsInside(cell))
        {
            return null;
        }

        return cells[cell.x, cell.y];
    }

    public bool IsEmpty(Vector2Int cell)
    {
        return IsInside(cell) && GetItem(cell) == null;
    }

    public void PlaceItem(Item item, Vector2Int cell)
    {
        if (!IsInside(cell))
        {
            return;
        }

        cells[cell.x, cell.y] = item;
    }

    public void RemoveItem(Vector2Int cell)
    {
        if (!IsInside(cell))
        {
            return;
        }

        cells[cell.x, cell.y] = null;
    }

    public bool HasEmptyCell()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cells[x, y] == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<Vector2Int> GetEmptyCells()
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cells[x, y] == null)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result;
    }

    public List<Vector2Int> FindNearestInCross(Vector2Int origin)
    {
        List<Vector2Int> found = new List<Vector2Int>();

        foreach (Vector2Int direction in Directions)
        {
            Vector2Int cell = origin + direction;

            while (IsInside(cell))
            {
                if (GetItem(cell) != null)
                {
                    found.Add(cell);
                    break;
                }

                cell += direction;
            }
        }

        return found;
    }

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
}
