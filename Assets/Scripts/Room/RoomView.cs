using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RoomView : MonoBehaviour
{
    [SerializeField] private int size = 5;
    [SerializeField] private int wallHeight = 5;
    [SerializeField] private float tileWidth = 2f;
    [SerializeField] private float tileHeight = 1f;
    [SerializeField] private float wallCellHeight = 1.2f;
    [SerializeField] private float lineWidth = 0.04f;
    [SerializeField] private bool showGrid = true;
    [SerializeField] private Color floorColor = new Color(0.85f, 0.65f, 0.45f);
    [SerializeField] private Color leftWallColor = new Color(0.62f, 0.75f, 0.85f);
    [SerializeField] private Color rightWallColor = new Color(0.72f, 0.84f, 0.92f);
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.5f);

    private RoomGrid grid;
    private IsoProjection projection;
    private Transform content;
    private GameObject gridRoot;
    private Material material;
    private bool gridVisible;

    private static readonly RoomSurface[] Surfaces = { RoomSurface.Floor, RoomSurface.LeftWall, RoomSurface.RightWall };

    private void Awake()
    {
        grid = new RoomGrid(size, wallHeight);
        projection = new IsoProjection(tileWidth, tileHeight, wallCellHeight);
        material = new Material(Shader.Find("Sprites/Default"));
        Build();
    }

    private void Update()
    {
        if (showGrid != gridVisible)
        {
            SetGridVisible(showGrid);
        }
    }

    public RoomGrid GetGrid()
    {
        return grid;
    }

    public void SetGridVisible(bool visible)
    {
        showGrid = visible;
        gridVisible = visible;
        gridRoot.SetActive(visible);
    }

    public Vector3 CellToWorld(RoomSurface surface, Vector2Int cell)
    {
        return content.TransformPoint(projection.CellCenter(surface, cell));
    }

    public Vector2Int WorldToFloorCell(Vector3 world)
    {
        return projection.FloorCellAt(content.InverseTransformPoint(world));
    }

    private void Build()
    {
        content = new GameObject("Content").transform;
        content.SetParent(transform, false);
        content.localPosition = -GetCenter();

        CreateMesh("Floor", BuildSurface(RoomSurface.Floor, floorColor), 0);
        CreateMesh("Left Wall", BuildSurface(RoomSurface.LeftWall, leftWallColor), 0);
        CreateMesh("Right Wall", BuildSurface(RoomSurface.RightWall, rightWallColor), 0);

        gridRoot = CreateMesh("Grid", BuildGrid(), 1);
        SetGridVisible(showGrid);
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

    private GameObject CreateMesh(string objectName, Mesh mesh, int sortingOrder)
    {
        GameObject meshObject = new GameObject(objectName);
        meshObject.transform.SetParent(content, false);
        meshObject.AddComponent<MeshFilter>().mesh = mesh;

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        meshRenderer.sortingOrder = sortingOrder;
        return meshObject;
    }

    private class MeshBuilder
    {
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Color> colors = new List<Color>();
        private readonly List<int> triangles = new List<int>();

        public void AddQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
        {
            int first = vertices.Count;
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);
            for (int i = 0; i < 4; i++)
            {
                colors.Add(color);
            }
            triangles.AddRange(new[] { first, first + 1, first + 2, first, first + 2, first + 3 });
        }

        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
