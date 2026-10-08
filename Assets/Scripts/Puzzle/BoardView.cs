using UnityEngine;

[DisallowMultipleComponent]
public class BoardView : MonoBehaviour
{
    [SerializeField] private Sprite boardSprite;
    [SerializeField] private Sprite[] itemSprites = new Sprite[6];
    [SerializeField] private float pixelsPerUnit = 16f;
    [SerializeField] private int cellPitch = 41;
    [SerializeField] private Vector2Int firstCellOffset = new Vector2Int(4, 3);
    [SerializeField] private int tileSize = 40;

    private Board board;
    private Transform itemsRoot;
    private SpriteRenderer boardRenderer;

    public void Show(Board board)
    {
        this.board = board;
        DrawBoard();
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
                    CreateItem(cell, item.GetItemType());
                }
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        Vector2 size = GetBoardPixelSize();
        int row = board.GetHeight() - 1 - cell.y;
        float px = firstCellOffset.x + cell.x * cellPitch + tileSize / 2f;
        float py = firstCellOffset.y + row * cellPitch + tileSize / 2f;
        Vector3 local = new Vector3((px - size.x / 2f) / pixelsPerUnit, (size.y / 2f - py) / pixelsPerUnit, 0f);
        return transform.position + local;
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        if (board == null)
        {
            return new Vector2Int(-1, -1);
        }

        Vector2 size = GetBoardPixelSize();
        Vector3 local = worldPosition - transform.position;
        float px = local.x * pixelsPerUnit + size.x / 2f - firstCellOffset.x;
        float py = size.y / 2f - local.y * pixelsPerUnit - firstCellOffset.y;
        int x = Mathf.FloorToInt(px / cellPitch);
        int row = Mathf.FloorToInt(py / cellPitch);
        return new Vector2Int(x, board.GetHeight() - 1 - row);
    }

    public bool IsInside(Vector2Int cell)
    {
        return board != null && board.IsInside(cell);
    }

    private Vector2 GetBoardPixelSize()
    {
        if (boardSprite != null)
        {
            return boardSprite.rect.size;
        }

        float width = firstCellOffset.x * 2 + board.GetWidth() * cellPitch - (cellPitch - tileSize);
        float height = firstCellOffset.y * 2 + board.GetHeight() * cellPitch - (cellPitch - tileSize);
        return new Vector2(width, height);
    }

    private void DrawBoard()
    {
        if (boardRenderer == null)
        {
            GameObject boardObject = new GameObject("Board Background");
            boardObject.transform.SetParent(transform, false);
            boardRenderer = boardObject.AddComponent<SpriteRenderer>();
            boardRenderer.sortingOrder = 0;
        }

        boardRenderer.sprite = boardSprite;
    }

    private void CreateItem(Vector2Int cell, ItemType type)
    {
        GameObject itemObject = new GameObject("Item " + cell.x + "," + cell.y);
        itemObject.transform.SetParent(itemsRoot, false);
        itemObject.transform.position = CellToWorld(cell);

        SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;

        Sprite sprite = GetSprite(type);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            spriteRenderer.sprite = SquareSprite.Get();
            spriteRenderer.color = GetColor(type);
            float size = tileSize / pixelsPerUnit;
            itemObject.transform.localScale = new Vector3(size, size, 1f);
        }
    }

    private Sprite GetSprite(ItemType type)
    {
        int index = (int)type;
        return itemSprites != null && index < itemSprites.Length ? itemSprites[index] : null;
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
