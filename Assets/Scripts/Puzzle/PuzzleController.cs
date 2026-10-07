using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoardView), typeof(BoardInput), typeof(LineView))]
[RequireComponent(typeof(HudView))]
[DisallowMultipleComponent]
public class PuzzleController : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 10;
    [SerializeField, Range(0f, 1f)] private float startFill = 0.4f;
    [SerializeField] private float linkShowTime = 0.15f;

    [SerializeField] private SpawnSettings spawnSettings = new SpawnSettings();

    private BoardView boardView;
    private BoardInput boardInput;
    private LineView lineView;
    private HudView hudView;
    private Wallet wallet;
    private PuzzleRound round;
    private bool isAnimating;

    private void Awake()
    {
        boardView = GetComponent<BoardView>();
        boardInput = GetComponent<BoardInput>();
        lineView = GetComponent<LineView>();
        hudView = GetComponent<HudView>();

        wallet = new Wallet();
        Spawner spawner = new Spawner(spawnSettings);
        round = new PuzzleRound(width, height, wallet, spawner);
        round.StartRound(startFill);
        boardView.Show(round.GetBoard());
    }

    private void OnEnable()
    {
        boardInput.Pressed += OnPress;
    }

    private void OnDisable()
    {
        boardInput.Pressed -= OnPress;
    }

    private void Update()
    {
        if (round.Update(Time.deltaTime))
        {
            boardView.Refresh();
        }
    }

    private void OnPress(Vector2Int cell)
    {
        if (isAnimating)
        {
            return;
        }

        TapResult result = round.Tap(cell);

        if (result.MatchedCells.Count > 0)
        {
            StartCoroutine(PlayMatch(cell, result));
        }
    }

    private IEnumerator PlayMatch(Vector2Int origin, TapResult result)
    {
        isAnimating = true;
        lineView.DrawLinks(origin, result.MatchedCells);

        yield return new WaitForSeconds(linkShowTime);

        lineView.Clear();
        boardView.Refresh();
        hudView.SetCoins(round.GetRoundCoins());

        if (result.IsCombo)
        {
            hudView.ShowCombo(result.Reward);
        }

        isAnimating = false;
    }
}
