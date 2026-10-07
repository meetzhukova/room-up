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

    public void DrawLinks(Vector2Int origin, IReadOnlyList<Vector2Int> targets)
    {
        Clear();

        dotsRoot = new GameObject("Links").transform;
        dotsRoot.SetParent(transform, false);

        Vector3 from = boardView.CellToWorld(origin);
        CreateDot(from);

        foreach (Vector2Int target in targets)
        {
            Vector3 to = boardView.CellToWorld(target);
            int distance = Mathf.Abs(target.x - origin.x) + Mathf.Abs(target.y - origin.y);
            int dotCount = distance * dotsPerCell;

            for (int step = 1; step <= dotCount; step++)
            {
                float t = (float)step / dotCount;
                CreateDot(Vector3.Lerp(from, to, t));
            }
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
