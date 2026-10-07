using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomView))]
[DisallowMultipleComponent]
public class FurnitureView : MonoBehaviour
{
    [SerializeField] private float rightSideShade = 0.8f;
    [SerializeField] private float leftSideShade = 0.65f;
    [SerializeField] private float frontMarkShade = 0.5f;
    [SerializeField] private float frontMarkWidth = 0.15f;
    [SerializeField] private float wallFrameWidth = 0.1f;
    [SerializeField] private float ghostAlpha = 0.75f;
    [SerializeField] private Color invalidColor = new Color(0.95f, 0.3f, 0.3f, 0.75f);

    private const int WallItemOrder = 5;
    private const int FirstFloorItemOrder = 10;
    private const int GhostOrder = 100;

    private RoomView roomView;
    private Transform itemsRoot;
    private GameObject ghost;

    private void Awake()
    {
        roomView = GetComponent<RoomView>();
    }

    public void Refresh(IReadOnlyList<PlacedFurniture> placed)
    {
        if (itemsRoot != null)
        {
            Destroy(itemsRoot.gameObject);
        }

        itemsRoot = new GameObject("Furniture").transform;
        itemsRoot.SetParent(roomView.GetContent(), false);

        List<PlacedFurniture> floorItems = new List<PlacedFurniture>();
        foreach (PlacedFurniture furniture in placed)
        {
            if (furniture.GetData().isWallItem)
            {
                roomView.CreateMesh(furniture.GetData().name, BuildMesh(furniture, furniture.GetData().color), WallItemOrder, itemsRoot);
            }
            else
            {
                floorItems.Add(furniture);
            }
        }

        List<PlacedFurniture> sorted = SortBackToFront(floorItems);
        for (int i = 0; i < sorted.Count; i++)
        {
            roomView.CreateMesh(sorted[i].GetData().name, BuildMesh(sorted[i], sorted[i].GetData().color), FirstFloorItemOrder + i, itemsRoot);
        }
    }

    public void ShowGhost(PlacedFurniture furniture, bool isValid)
    {
        HideGhost();

        Color color = furniture.GetData().color;
        color.a = ghostAlpha;
        if (!isValid)
        {
            color = invalidColor;
        }

        ghost = roomView.CreateMesh("Ghost", BuildMesh(furniture, color), GhostOrder, roomView.GetContent());
    }

    public void HideGhost()
    {
        if (ghost != null)
        {
            Destroy(ghost);
            ghost = null;
        }
    }

    private Mesh BuildMesh(PlacedFurniture furniture, Color color)
    {
        MeshBuilder builder = new MeshBuilder();

        if (furniture.GetData().isWallItem)
        {
            AddWallPanel(builder, furniture, color);
        }
        else
        {
            AddBox(builder, furniture, color);
        }

        return builder.ToMesh();
    }

    private void AddWallPanel(MeshBuilder builder, PlacedFurniture furniture, Color color)
    {
        RoomSurface surface = furniture.GetSurface();
        Vector2 min = furniture.GetOrigin();
        Vector2 max = min + (Vector2)furniture.GetSize();
        Vector2 inset = new Vector2(wallFrameWidth, wallFrameWidth);

        AddSurfaceRect(builder, surface, min, max, Shade(color, frontMarkShade));
        AddSurfaceRect(builder, surface, min + inset, max - inset, color);
    }

    private void AddSurfaceRect(MeshBuilder builder, RoomSurface surface, Vector2 min, Vector2 max, Color color)
    {
        IsoProjection projection = roomView.GetProjection();
        builder.AddQuad(
            projection.ToWorld(surface, new Vector2(min.x, min.y)),
            projection.ToWorld(surface, new Vector2(max.x, min.y)),
            projection.ToWorld(surface, new Vector2(max.x, max.y)),
            projection.ToWorld(surface, new Vector2(min.x, max.y)),
            color);
    }

    private void AddBox(MeshBuilder builder, PlacedFurniture furniture, Color color)
    {
        Vector2Int origin = furniture.GetOrigin();
        Vector2Int size = furniture.GetSize();
        float x0 = origin.x;
        float y0 = origin.y;
        float x1 = origin.x + size.x;
        float y1 = origin.y + size.y;
        float h = furniture.GetData().height;

        builder.AddQuad(P(x1, y0, 0f), P(x1, y1, 0f), P(x1, y1, h), P(x1, y0, h), Shade(color, rightSideShade));
        builder.AddQuad(P(x0, y1, 0f), P(x1, y1, 0f), P(x1, y1, h), P(x0, y1, h), Shade(color, leftSideShade));
        builder.AddQuad(P(x0, y0, h), P(x1, y0, h), P(x1, y1, h), P(x0, y1, h), color);

        AddFrontMark(builder, furniture.GetRotation(), x0, y0, x1, y1, h, Shade(color, frontMarkShade));
    }

    private void AddFrontMark(MeshBuilder builder, int rotation, float x0, float y0, float x1, float y1, float h, Color color)
    {
        float w = frontMarkWidth;
        switch (rotation)
        {
            case 0:
                builder.AddQuad(P(x0, y1 - w, h), P(x1, y1 - w, h), P(x1, y1, h), P(x0, y1, h), color);
                break;
            case 1:
                builder.AddQuad(P(x1 - w, y0, h), P(x1, y0, h), P(x1, y1, h), P(x1 - w, y1, h), color);
                break;
            case 2:
                builder.AddQuad(P(x0, y0, h), P(x1, y0, h), P(x1, y0 + w, h), P(x0, y0 + w, h), color);
                break;
            default:
                builder.AddQuad(P(x0, y0, h), P(x0 + w, y0, h), P(x0 + w, y1, h), P(x0, y1, h), color);
                break;
        }
    }

    private Vector2 P(float x, float y, float z)
    {
        return roomView.GetProjection().ToWorld(new Vector3(x, y, z));
    }

    private static Color Shade(Color color, float amount)
    {
        return new Color(color.r * amount, color.g * amount, color.b * amount, color.a);
    }

    private static List<PlacedFurniture> SortBackToFront(List<PlacedFurniture> items)
    {
        List<PlacedFurniture> remaining = new List<PlacedFurniture>(items);
        List<PlacedFurniture> sorted = new List<PlacedFurniture>();

        while (remaining.Count > 0)
        {
            PlacedFurniture next = remaining[0];
            foreach (PlacedFurniture candidate in remaining)
            {
                if (!HasItemBehind(candidate, remaining))
                {
                    next = candidate;
                    break;
                }
            }

            remaining.Remove(next);
            sorted.Add(next);
        }

        return sorted;
    }

    private static bool HasItemBehind(PlacedFurniture item, List<PlacedFurniture> others)
    {
        foreach (PlacedFurniture other in others)
        {
            if (other != item && IsBehind(other, item))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsBehind(PlacedFurniture back, PlacedFurniture front)
    {
        Vector2Int backMin = back.GetOrigin();
        Vector2Int backMax = backMin + back.GetSize();
        Vector2Int frontMin = front.GetOrigin();
        Vector2Int frontMax = frontMin + front.GetSize();

        bool inFrontAlongX = frontMin.x >= backMax.x && frontMax.y > backMin.y;
        bool inFrontAlongY = frontMin.y >= backMax.y && frontMax.x > backMin.x;
        return inFrontAlongX || inFrontAlongY;
    }
}
