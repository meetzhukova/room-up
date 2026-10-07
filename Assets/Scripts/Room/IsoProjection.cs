using UnityEngine;

public class IsoProjection
{
    private readonly float halfTileWidth;
    private readonly float halfTileHeight;
    private readonly float wallCellHeight;

    public IsoProjection(float tileWidth, float tileHeight, float wallCellHeight)
    {
        halfTileWidth = tileWidth / 2f;
        halfTileHeight = tileHeight / 2f;
        this.wallCellHeight = wallCellHeight;
    }

    public Vector2 ToWorld(RoomSurface surface, Vector2 point)
    {
        switch (surface)
        {
            case RoomSurface.LeftWall:
                return new Vector2(-point.x * halfTileWidth, -point.x * halfTileHeight + point.y * wallCellHeight);
            case RoomSurface.RightWall:
                return new Vector2(point.x * halfTileWidth, -point.x * halfTileHeight + point.y * wallCellHeight);
            default:
                return new Vector2((point.x - point.y) * halfTileWidth, -(point.x + point.y) * halfTileHeight);
        }
    }

    public Vector2 CellCenter(RoomSurface surface, Vector2Int cell)
    {
        return ToWorld(surface, new Vector2(cell.x + 0.5f, cell.y + 0.5f));
    }

    public Vector2Int FloorCellAt(Vector2 world)
    {
        float difference = world.x / halfTileWidth;
        float sum = -world.y / halfTileHeight;
        float x = (sum + difference) / 2f;
        float y = (sum - difference) / 2f;
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }
}
