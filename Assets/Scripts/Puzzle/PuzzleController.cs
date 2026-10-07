using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoardView), typeof(BoardInput), typeof(LineView))]
[RequireComponent(typeof(HudView), typeof(ResultsView))]
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
    private ResultsView resultsView;
    private Wallet wallet;
    private PuzzleRound round;
    private bool isAnimating;

    private void Awake()
    {
        boardView = GetComponent<BoardView>();
        boardInput = GetComponent<BoardInput>();
        lineView = GetComponent<LineView>();
        hudView = GetComponent<HudView>();
        resultsView = GetComponent<ResultsView>();

        wallet = Game.Progress.GetWallet();
    }

    private void Start()
    {
        StartNewRound();
    }

    private void OnEnable()
    {
        boardInput.Pressed += OnPress;
        resultsView.PlayAgainClicked += StartNewRound;
        resultsView.HomeClicked += GoHome;
    }

    private void OnDisable()
    {
        boardInput.Pressed -= OnPress;
        resultsView.PlayAgainClicked -= StartNewRound;
        resultsView.HomeClicked -= GoHome;
    }

    private void Update()
    {
        if (round == null || round.IsOver())
        {
            return;
        }

        if (round.Update(Time.deltaTime))
        {
            boardView.Refresh();
        }

        if (round.IsOver())
        {
            EndRound();
        }
    }

    private void StartNewRound()
    {
        StopAllCoroutines();
        isAnimating = false;
        lineView.Clear();

        Spawner spawner = new Spawner(spawnSettings);
        round = new PuzzleRound(width, height, wallet, spawner);
        round.StartRound(startFill);

        boardView.Show(round.GetBoard());
        hudView.SetCoins(0);
        resultsView.Hide();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Game.Save();
        }
    }

    private void OnApplicationQuit()
    {
        Game.Save();
    }

    private void GoHome()
    {
        Game.OpenRoom();
    }

    private void EndRound()
    {
        PlayerProgress progress = Game.Progress;
        int roundCoins = round.GetRoundCoins();
        bool isNewBest = progress.TrySetBestRoundCoins(roundCoins);
        Game.Save();

        resultsView.Show(roundCoins, progress.GetBestRoundCoins(), isNewBest);
    }

    private void OnPress(Vector2Int cell)
    {
        if (isAnimating || round == null || round.IsOver())
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
