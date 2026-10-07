using System.Collections.Generic;
using UnityEngine;

public class RoomGrid
{
    private readonly int size;
    private readonly int wallHeight;
    private readonly Dictionary<RoomSurface, PlacedFurniture[,]> cells = new Dictionary<RoomSurface, PlacedFurniture[,]>();
    private readonly List<PlacedFurniture> placed = new List<PlacedFurniture>();

    public RoomGrid(int size, int wallHeight)
    {
        this.size = size;
        this.wallHeight = wallHeight;

        cells[RoomSurface.Floor] = new PlacedFurniture[size, size];
        cells[RoomSurface.LeftWall] = new PlacedFurniture[size, wallHeight];
        cells[RoomSurface.RightWall] = new PlacedFurniture[size, wallHeight];
    }

    public int GetWidth(RoomSurface surface)
    {
        return size;
    }

    public int GetHeight(RoomSurface surface)
    {
        return surface == RoomSurface.Floor ? size : wallHeight;
    }

    public bool IsInside(RoomSurface surface, Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < GetWidth(surface)
            && cell.y >= 0 && cell.y < GetHeight(surface);
    }

    public PlacedFurniture GetAt(RoomSurface surface, Vector2Int cell)
    {
        return IsInside(surface, cell) ? cells[surface][cell.x, cell.y] : null;
    }

    public IReadOnlyList<PlacedFurniture> GetPlaced()
    {
        return placed;
    }

    public bool CanPlace(PlacedFurniture furniture)
    {
        bool onFloor = furniture.GetSurface() == RoomSurface.Floor;
        if (furniture.GetData().isWallItem == onFloor)
        {
            return false;
        }

        foreach (Vector2Int cell in furniture.GetCells())
        {
            if (!IsInside(furniture.GetSurface(), cell) || GetAt(furniture.GetSurface(), cell) != null)
            {
                return false;
            }
        }

        return true;
    }

    public bool Place(PlacedFurniture furniture)
    {
        if (!CanPlace(furniture))
        {
            return false;
        }

        SetCells(furniture, furniture);
        placed.Add(furniture);
        return true;
    }

    public void Remove(PlacedFurniture furniture)
    {
        if (!placed.Remove(furniture))
        {
            return;
        }

        SetCells(furniture, null);
    }

    public Vector2Int ClampOrigin(RoomSurface surface, Vector2Int origin, Vector2Int footprint)
    {
        int x = Mathf.Clamp(origin.x, 0, GetWidth(surface) - footprint.x);
        int y = Mathf.Clamp(origin.y, 0, GetHeight(surface) - footprint.y);
        return new Vector2Int(x, y);
    }

    private void SetCells(PlacedFurniture furniture, PlacedFurniture value)
    {
        foreach (Vector2Int cell in furniture.GetCells())
        {
            cells[furniture.GetSurface()][cell.x, cell.y] = value;
        }
    }
}
