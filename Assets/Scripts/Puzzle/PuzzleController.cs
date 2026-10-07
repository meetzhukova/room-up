using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardView), typeof(BoardInput), typeof(LineView))]
public class PuzzleController : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 10;
    [SerializeField, Range(0f, 1f)] private float startFill = 0.4f;
    [SerializeField] private float linkShowTime = 0.15f;

    private BoardView boardView;
    private BoardInput boardInput;
    private LineView lineView;
    private PuzzleRound round;
    private bool isAnimating;

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
    }

    private void OnDisable()
    {
        boardInput.Pressed -= OnPress;
    }

    private void OnPress(Vector2Int cell)
    {
        if (isAnimating)
        {
            return;
        }

        List<Vector2Int> matched = round.Tap(cell);

        if (matched.Count > 0)
        {
            StartCoroutine(PlayMatch(cell, matched));
        }
    }

    private IEnumerator PlayMatch(Vector2Int origin, List<Vector2Int> matched)
    {
        isAnimating = true;
        lineView.DrawLinks(origin, matched);

        yield return new WaitForSeconds(linkShowTime);

        lineView.Clear();
        boardView.Refresh();
        isAnimating = false;
    }
}
