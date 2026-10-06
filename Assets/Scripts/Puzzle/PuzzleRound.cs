using UnityEngine;

public class PuzzleRound
{
    private Board board;
    private Line line = new Line();

    public PuzzleRound(int width, int height)
    {
        board = new Board(width, height);
    }

    public Board GetBoard()
    {
        return board;
    }

    public Line GetLine()
    {
        return line;
    }

    public void StartRound(float startFill)
    {
        int itemCount = Mathf.RoundToInt(board.GetWidth() * board.GetHeight() * startFill);
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

    public void OnPress(Vector2Int cell)
    {
        if (board.GetItem(cell) == null)
        {
            return;
        }

        line.Start(cell);
    }

    public void OnDrag(Vector2Int cell)
    {
        if (!line.IsActive())
        {
            return;
        }

        if (line.IsPreviousCell(cell))
        {
            line.TryExtend(cell);
            return;
        }

        if (HasReachedItem())
        {
            return;
        }

        Item startItem = board.GetItem(line.GetStart());
        Item item = board.GetItem(cell);

        if (item != null && !item.IsSameType(startItem))
        {
            return;
        }

        line.TryExtend(cell);
    }

    public bool OnRelease(Vector2Int cell)
    {
        bool matched = false;

        if (line.IsActive() && line.GetLength() > 1)
        {
            matched = TryMatch(line.GetStart(), line.GetEnd());
        }

        line.Clear();
        return matched;
    }

    private bool TryMatch(Vector2Int a, Vector2Int b)
    {
        Item first = board.GetItem(a);
        Item second = board.GetItem(b);

        if (first == null || !first.IsSameType(second))
        {
            return false;
        }

        board.RemoveItem(a);
        board.RemoveItem(b);
        return true;
    }

    private bool HasReachedItem()
    {
        return line.GetLength() > 1 && board.GetItem(line.GetEnd()) != null;
    }
}
