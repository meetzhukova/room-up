using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RoomView))]
[DisallowMultipleComponent]
public class RoomInput : MonoBehaviour
{
    public event Action<Vector3> Pressed;
    public event Action<Vector3> Dragged;
    public event Action<Vector3> Released;

    private RoomView roomView;
    private Camera mainCamera;
    private bool isPressing;
    private Vector2 lastScreenPosition;

    private void Awake()
    {
        roomView = GetComponent<RoomView>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null)
        {
            return;
        }

        Vector2 screenPosition = pointer.position.ReadValue();
        Vector3 world = ScreenToWorld(screenPosition);

        if (pointer.press.wasPressedThisFrame)
        {
            if (IsOverUi() || !roomView.TryGetCell(world, out _, out _))
            {
                return;
            }

            isPressing = true;
            lastScreenPosition = screenPosition;
            Pressed?.Invoke(world);
        }
        else if (isPressing && pointer.press.isPressed)
        {
            if (screenPosition != lastScreenPosition)
            {
                lastScreenPosition = screenPosition;
                Dragged?.Invoke(world);
            }
        }
        else if (isPressing && pointer.press.wasReleasedThisFrame)
        {
            isPressing = false;
            Released?.Invoke(world);
        }
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        Vector3 world = mainCamera.ScreenToWorldPoint(screenPosition);
        world.z = 0f;
        return world;
    }

    private static bool IsOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
