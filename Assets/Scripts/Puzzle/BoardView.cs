using UnityEngine;

[DisallowMultipleComponent]
public class BoardView : MonoBehaviour
{
    [SerializeField] private float cellSize = 2.5f;
    [SerializeField] private Color cellColor = new Color(0.22f, 0.24f, 0.33f);

    private Board board;
    private Transform cellsRoot;
    private Transform itemsRoot;

    public void Show(Board board)
    {
        this.board = board;
        DrawCells();
        Refresh();
    }

    public void Refresh()
    {
        if (itemsRoot != null)
        {
            Destroy(itemsRoot.gameObject);
        }

        itemsRoot = new GameObject("Items").transform;
        itemsRoot.SetParent(transform, false);

        for (int x = 0; x < board.GetWidth(); x++)
        {
            for (int y = 0; y < board.GetHeight(); y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Item item = board.GetItem(cell);

                if (item != null)
                {
                    CreateSquare("Item " + x + "," + y, itemsRoot, CellToWorld(cell),
                        cellSize * 0.7f, GetColor(item.GetItemType()), 1);
                }
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        float offsetX = (cell.x - (board.GetWidth() - 1) / 2f) * cellSize;
        float offsetY = (cell.y - (board.GetHeight() - 1) / 2f) * cellSize;
        return transform.position + new Vector3(offsetX, offsetY, 0f);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        if (board == null)
        {
            return new Vector2Int(-1, -1);
        }

        Vector3 local = worldPosition - transform.position;
        int x = Mathf.RoundToInt(local.x / cellSize + (board.GetWidth() - 1) / 2f);
        int y = Mathf.RoundToInt(local.y / cellSize + (board.GetHeight() - 1) / 2f);
        return new Vector2Int(x, y);
    }

    public bool IsInside(Vector2Int cell)
    {
        return board != null && board.IsInside(cell);
    }

    private void DrawCells()
    {
        if (cellsRoot != null)
        {
            Destroy(cellsRoot.gameObject);
        }

        cellsRoot = new GameObject("Cells").transform;
        cellsRoot.SetParent(transform, false);

        for (int x = 0; x < board.GetWidth(); x++)
        {
            for (int y = 0; y < board.GetHeight(); y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                CreateSquare("Cell " + x + "," + y, cellsRoot, CellToWorld(cell),
                    cellSize * 0.92f, cellColor, 0);
            }
        }
    }

    private void CreateSquare(string objectName, Transform parent, Vector3 position,
        float size, Color color, int sortingOrder)
    {
        GameObject square = new GameObject(objectName);
        square.transform.SetParent(parent, false);
        square.transform.position = position;
        square.transform.localScale = new Vector3(size, size, 1f);

        SpriteRenderer spriteRenderer = square.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SquareSprite.Get();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private static Color GetColor(ItemType type)
    {
        switch (type)
        {
            case ItemType.Red: return new Color(0.94f, 0.33f, 0.33f);
            case ItemType.Blue: return new Color(0.29f, 0.53f, 0.96f);
            case ItemType.Green: return new Color(0.36f, 0.80f, 0.40f);
            case ItemType.Yellow: return new Color(0.98f, 0.84f, 0.28f);
            case ItemType.Purple: return new Color(0.66f, 0.42f, 0.92f);
            case ItemType.Orange: return new Color(0.98f, 0.58f, 0.24f);
            default: return Color.white;
        }
    }
}
