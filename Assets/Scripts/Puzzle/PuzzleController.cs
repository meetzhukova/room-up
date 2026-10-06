using UnityEngine;

[RequireComponent(typeof(BoardView), typeof(BoardInput), typeof(LineView))]
public class PuzzleController : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 10;
    [SerializeField, Range(0f, 1f)] private float startFill = 0.4f;

    private BoardView boardView;
    private BoardInput boardInput;
    private LineView lineView;
    private PuzzleRound round;

    private void Awake()
    {
        boardView = GetComponent<BoardView>();
        boardInput = GetComponent<BoardInput>();
        lineView = GetComponent<LineView>();

        round = new PuzzleRound(width, height);
        round.StartRound(startFill);
        boardView.Show(round.GetBoard());
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
        round.OnPress(cell);
        lineView.Draw(round.GetLine().GetCells());
    }

    private void OnDrag(Vector2Int cell)
    {
        round.OnDrag(cell);
        lineView.Draw(round.GetLine().GetCells());
    }

    private void OnRelease(Vector2Int cell)
    {
        bool matched = round.OnRelease(cell);
        lineView.Clear();

        if (matched)
        {
            boardView.Refresh();
        }
    }
}
