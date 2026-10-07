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

    public Vector2 ToWorld(Vector3 roomPoint)
    {
        float x = (roomPoint.x - roomPoint.y) * halfTileWidth;
        float y = -(roomPoint.x + roomPoint.y) * halfTileHeight + roomPoint.z * wallCellHeight;
        return new Vector2(x, y);
    }

    public Vector2 ToWorld(RoomSurface surface, Vector2 point)
    {
        return ToWorld(ToRoomPoint(surface, point));
    }

    public Vector2 CellCenter(RoomSurface surface, Vector2Int cell)
    {
        return ToWorld(surface, new Vector2(cell.x + 0.5f, cell.y + 0.5f));
    }

    public Vector2Int CellAt(RoomSurface surface, Vector2 world)
    {
        switch (surface)
        {
            case RoomSurface.LeftWall:
                return WallCellAt(-world.x / halfTileWidth, world.y);
            case RoomSurface.RightWall:
                return WallCellAt(world.x / halfTileWidth, world.y);
            default:
                return FloorCellAt(world);
        }
    }

    public static Vector3 ToRoomPoint(RoomSurface surface, Vector2 point)
    {
        switch (surface)
        {
            case RoomSurface.LeftWall:
                return new Vector3(0f, point.x, point.y);
            case RoomSurface.RightWall:
                return new Vector3(point.x, 0f, point.y);
            default:
                return new Vector3(point.x, point.y, 0f);
        }
    }

    private Vector2Int WallCellAt(float along, float worldY)
    {
        float up = (worldY + along * halfTileHeight) / wallCellHeight;
        return new Vector2Int(Mathf.FloorToInt(along), Mathf.FloorToInt(up));
    }

    private Vector2Int FloorCellAt(Vector2 world)
    {
        float difference = world.x / halfTileWidth;
        float sum = -world.y / halfTileHeight;
        float x = (sum + difference) / 2f;
        float y = (sum - difference) / 2f;
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }
}
