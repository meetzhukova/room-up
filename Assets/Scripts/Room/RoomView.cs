using UnityEngine;

[DisallowMultipleComponent]
public class RoomView : MonoBehaviour
{
    [SerializeField] private float tileWidth = 2f;
    [SerializeField] private float tileHeight = 1f;
    [SerializeField] private float wallCellHeight = 1.2f;
    [SerializeField] private float lineWidth = 0.04f;
    [SerializeField] private Color floorColor = new Color(0.85f, 0.65f, 0.45f);
    [SerializeField] private Color leftWallColor = new Color(0.62f, 0.75f, 0.85f);
    [SerializeField] private Color rightWallColor = new Color(0.72f, 0.84f, 0.92f);
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.5f);

    private RoomGrid grid;
    private IsoProjection projection;
    private Transform content;
    private GameObject gridRoot;
    private Material material;

    private static readonly RoomSurface[] Surfaces = { RoomSurface.Floor, RoomSurface.LeftWall, RoomSurface.RightWall };

    private void Awake()
    {
        projection = new IsoProjection(tileWidth, tileHeight, wallCellHeight);
        material = new Material(Shader.Find("Sprites/Default"));
    }

    public void Show(RoomGrid roomGrid)
    {
        grid = roomGrid;

        if (content != null)
        {
            Destroy(content.gameObject);
        }

        content = new GameObject("Content").transform;
        content.SetParent(transform, false);
        content.localPosition = -GetCenter();

        CreateMesh("Floor", BuildSurface(RoomSurface.Floor, floorColor), 0, content);
        CreateMesh("Left Wall", BuildSurface(RoomSurface.LeftWall, leftWallColor), 0, content);
        CreateMesh("Right Wall", BuildSurface(RoomSurface.RightWall, rightWallColor), 0, content);

        gridRoot = CreateMesh("Grid", BuildGrid(), 1, content);
        SetGridVisible(false);
    }

    public IsoProjection GetProjection()
    {
        return projection;
    }

    public Transform GetContent()
    {
        return content;
    }

    public void SetGridVisible(bool visible)
    {
        gridRoot.SetActive(visible);
    }

    public bool TryGetCell(Vector3 world, out RoomSurface surface, out Vector2Int cell)
    {
        foreach (RoomSurface candidate in Surfaces)
        {
            Vector2Int candidateCell = GetCellOn(candidate, world);
            if (grid.IsInside(candidate, candidateCell) && IsOnSide(candidate, world))
            {
                surface = candidate;
                cell = candidateCell;
                return true;
            }
        }

        surface = RoomSurface.Floor;
        cell = Vector2Int.zero;
        return false;
    }

    public Vector2Int GetCellOn(RoomSurface surface, Vector3 world)
    {
        return projection.CellAt(surface, content.InverseTransformPoint(world));
    }

    public RoomSurface GetNearestWall(Vector3 world)
    {
        return content.InverseTransformPoint(world).x < 0f ? RoomSurface.LeftWall : RoomSurface.RightWall;
    }

    public GameObject CreateMesh(string objectName, Mesh mesh, int sortingOrder, Transform parent)
    {
        GameObject meshObject = new GameObject(objectName);
        meshObject.transform.SetParent(parent, false);
        meshObject.AddComponent<MeshFilter>().mesh = mesh;

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        meshRenderer.sortingOrder = sortingOrder;
        return meshObject;
    }

    private bool IsOnSide(RoomSurface surface, Vector3 world)
    {
        return surface == RoomSurface.Floor || GetNearestWall(world) == surface;
    }

    private Vector3 GetCenter()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (RoomSurface surface in Surfaces)
        {
            foreach (Vector2 corner in GetCorners(surface))
            {
                bounds.Encapsulate(corner);
            }
        }
        return bounds.center;
    }

    private Vector2[] GetCorners(RoomSurface surface)
    {
        int width = grid.GetWidth(surface);
        int height = grid.GetHeight(surface);
        return new[]
        {
            projection.ToWorld(surface, new Vector2(0, 0)),
            projection.ToWorld(surface, new Vector2(width, 0)),
            projection.ToWorld(surface, new Vector2(width, height)),
            projection.ToWorld(surface, new Vector2(0, height))
        };
    }

    private Mesh BuildSurface(RoomSurface surface, Color color)
    {
        MeshBuilder builder = new MeshBuilder();
        Vector2[] corners = GetCorners(surface);
        builder.AddQuad(corners[0], corners[1], corners[2], corners[3], color);
        return builder.ToMesh();
    }

    private Mesh BuildGrid()
    {
        MeshBuilder builder = new MeshBuilder();
        foreach (RoomSurface surface in Surfaces)
        {
            int width = grid.GetWidth(surface);
            int height = grid.GetHeight(surface);

            for (int x = 0; x <= width; x++)
            {
                AddLine(builder, surface, new Vector2(x, 0), new Vector2(x, height));
            }

            for (int y = 0; y <= height; y++)
            {
                AddLine(builder, surface, new Vector2(0, y), new Vector2(width, y));
            }
        }
        return builder.ToMesh();
    }

    private void AddLine(MeshBuilder builder, RoomSurface surface, Vector2 from, Vector2 to)
    {
        Vector2 start = projection.ToWorld(surface, from);
        Vector2 end = projection.ToWorld(surface, to);
        Vector2 direction = (end - start).normalized;
        Vector2 offset = new Vector2(-direction.y, direction.x) * (lineWidth / 2f);
        builder.AddQuad(start - offset, end - offset, end + offset, start + offset, gridColor);
    }
}
