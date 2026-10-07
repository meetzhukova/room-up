using System.Collections.Generic;
using UnityEngine;

public class PlacedFurniture
{
    private readonly FurnitureData data;
    private RoomSurface surface;
    private Vector2Int origin;
    private int rotation;

    public PlacedFurniture(FurnitureData data, RoomSurface surface, Vector2Int origin)
    {
        this.data = data;
        this.surface = surface;
        this.origin = origin;
    }

    public FurnitureData GetData()
    {
        return data;
    }

    public RoomSurface GetSurface()
    {
        return surface;
    }

    public Vector2Int GetOrigin()
    {
        return origin;
    }

    public int GetRotation()
    {
        return rotation;
    }

    public Vector2Int GetSize()
    {
        return rotation % 2 == 0 ? data.size : new Vector2Int(data.size.y, data.size.x);
    }

    public void MoveTo(RoomSurface newSurface, Vector2Int newOrigin)
    {
        surface = newSurface;
        origin = newOrigin;
    }

    public void Rotate()
    {
        if (data.isWallItem)
        {
            return;
        }

        rotation = (rotation + 1) % 4;
    }

    public List<Vector2Int> GetCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        Vector2Int size = GetSize();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                cells.Add(origin + new Vector2Int(x, y));
            }
        }

        return cells;
    }
}
