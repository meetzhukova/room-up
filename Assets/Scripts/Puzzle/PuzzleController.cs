using UnityEngine;

[RequireComponent(typeof(BoardView), typeof(BoardInput), typeof(LineView))]
public class PuzzleController : MonoBehaviour
{
    private BoardView boardView;
    private BoardInput boardInput;
    private LineView lineView;
    private Line line = new Line();

    private void Awake()
    {
        boardView = GetComponent<BoardView>();
        boardInput = GetComponent<BoardInput>();
        lineView = GetComponent<LineView>();
    }

    private void OnEnable()
    {
        boardInput.Pressed += OnPress;
        boardInput.Dragged += OnDrag;
        boardInput.Released += OnRelease;
    }

    private void OnDisable()
    {
        boardInput.Pressed -= OnPress;
        boardInput.Dragged -= OnDrag;
        boardInput.Released -= OnRelease;
    }

    private void OnPress(Vector2Int cell)
    {
        if (boardView.GetItem(cell) == null)
        {
            return;
        }

        line.Start(cell);
        lineView.Draw(line.GetCells());
    }

    private void OnDrag(Vector2Int cell)
    {
        if (!line.IsActive())
        {
            return;
        }

        if (line.IsPreviousCell(cell))
        {
            line.TryExtend(cell);
            lineView.Draw(line.GetCells());
            return;
        }

        if (HasReachedItem())
        {
            return;
        }

        Item startItem = boardView.GetItem(line.GetStart());
        Item item = boardView.GetItem(cell);

        if (item != null && !item.IsSameType(startItem))
        {
            return;
        }

        if (line.TryExtend(cell))
        {
            lineView.Draw(line.GetCells());
        }
    }

    private void OnRelease(Vector2Int cell)
    {
        line.Clear();
        lineView.Clear();
    }

    private bool HasReachedItem()
    {
        return line.GetLength() > 1 && boardView.GetItem(line.GetEnd()) != null;
    }
}
