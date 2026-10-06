using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardView))]
public class LineView : MonoBehaviour
{
    [SerializeField] private float dotSize = 0.45f;
    [SerializeField] private int dotsPerCell = 3;
    [SerializeField] private Color dotColor = Color.white;

    private BoardView boardView;
    private Transform dotsRoot;

    private void Awake()
    {
        boardView = GetComponent<BoardView>();
    }

    public void Draw(IReadOnlyList<Vector2Int> cells)
    {
        Clear();

        dotsRoot = new GameObject("Line").transform;
        dotsRoot.SetParent(transform, false);

        for (int i = 0; i < cells.Count - 1; i++)
        {
            Vector3 from = boardView.CellToWorld(cells[i]);
            Vector3 to = boardView.CellToWorld(cells[i + 1]);

            for (int step = 0; step < dotsPerCell; step++)
            {
                float t = (float)step / dotsPerCell;
                CreateDot(Vector3.Lerp(from, to, t));
            }
        }

        if (cells.Count > 0)
        {
            CreateDot(boardView.CellToWorld(cells[cells.Count - 1]));
        }
    }

    public void Clear()
    {
        if (dotsRoot != null)
        {
            Destroy(dotsRoot.gameObject);
            dotsRoot = null;
        }
    }

    private void CreateDot(Vector3 position)
    {
        GameObject dot = new GameObject("Dot");
        dot.transform.SetParent(dotsRoot, false);
        dot.transform.position = position;
        dot.transform.localScale = new Vector3(dotSize, dotSize, 1f);

        SpriteRenderer spriteRenderer = dot.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SquareSprite.Get();
        spriteRenderer.color = dotColor;
        spriteRenderer.sortingOrder = 2;
    }
}
