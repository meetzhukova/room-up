using UnityEngine;

public class RoomGrid
{
    private readonly int size;
    private readonly int wallHeight;

    public RoomGrid(int size, int wallHeight)
    {
        this.size = size;
        this.wallHeight = wallHeight;
    }

    public int GetSize()
    {
        return size;
    }

    public int GetWallHeight()
    {
        return wallHeight;
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
}
