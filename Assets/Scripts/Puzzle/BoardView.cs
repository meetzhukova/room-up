using UnityEngine;

public class BoardView : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 10;
    [SerializeField, Range(0f, 1f)] private float startFill = 0.4f;

    [Header("Look")]
    [SerializeField] private float cellSize = 2.5f;
    [SerializeField] private Color cellColor = new Color(0.22f, 0.24f, 0.33f);

    private Board board;
    private Sprite squareSprite;
    private Transform itemsRoot;

    private void Start()
    {
        squareSprite = CreateSquareSprite();
        board = new Board(width, height);

        FillRandom();
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

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        float offsetX = (cell.x - (width - 1) / 2f) * cellSize;
        float offsetY = (cell.y - (height - 1) / 2f) * cellSize;
        return transform.position + new Vector3(offsetX, offsetY, 0f);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        Vector3 local = worldPosition - transform.position;
        int x = Mathf.RoundToInt(local.x / cellSize + (width - 1) / 2f);
        int y = Mathf.RoundToInt(local.y / cellSize + (height - 1) / 2f);
        return new Vector2Int(x, y);
    }

    public bool IsInside(Vector2Int cell)
    {
        return board.IsInside(cell);
    }

    private void FillRandom()
    {
        int itemCount = Mathf.RoundToInt(width * height * startFill);
        int typeCount = System.Enum.GetValues(typeof(ItemType)).Length;

        for (int i = 0; i < itemCount; i++)
        {
            var emptyCells = board.GetEmptyCells();
            if (emptyCells.Count == 0)
            {
                return;
            }

            Vector2Int cell = emptyCells[Random.Range(0, emptyCells.Count)];
            ItemType type = (ItemType)Random.Range(0, typeCount);
            board.PlaceItem(new Item(type), cell);
        }
    }

    private void DrawCells()
    {
        Transform cellsRoot = new GameObject("Cells").transform;
        cellsRoot.SetParent(transform, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        spriteRenderer.sprite = squareSprite;
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private static Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
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
