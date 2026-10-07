using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
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
