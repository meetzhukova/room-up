using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoardView))]
[DisallowMultipleComponent]
public class BoardInput : MonoBehaviour
{
    public event Action<Vector2Int> Pressed;
    public event Action<Vector2Int> Dragged;
    public event Action<Vector2Int> Released;

    [SerializeField] private bool logToConsole = true;

    private BoardView boardView;
    private Camera mainCamera;
    private bool isPressing;
    private Vector2Int lastCell;

    private void Awake()
    {
        boardView = GetComponent<BoardView>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null)
        {
            return;
        }

        Vector2Int cell = ScreenToCell(pointer.position.ReadValue());

        if (pointer.press.wasPressedThisFrame)
        {
            if (!boardView.IsInside(cell))
            {
                return;
            }

            isPressing = true;
            lastCell = cell;
            Report("Press", cell, Pressed);
        }
        else if (isPressing && pointer.press.isPressed)
        {
            if (cell != lastCell && boardView.IsInside(cell))
            {
                lastCell = cell;
                Report("Drag", cell, Dragged);
            }
        }
        else if (isPressing && pointer.press.wasReleasedThisFrame)
        {
            isPressing = false;
            Report("Release", cell, Released);
        }
    }

    private Vector2Int ScreenToCell(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;
        return boardView.WorldToCell(worldPosition);
    }

    private void Report(string moment, Vector2Int cell, Action<Vector2Int> listeners)
    {
        if (logToConsole)
        {
            Debug.Log(moment + " " + cell);
        }

        listeners?.Invoke(cell);
    }
}
