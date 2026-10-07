using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomView), typeof(FurnitureView), typeof(RoomInput))]
[RequireComponent(typeof(RoomHud))]
[DisallowMultipleComponent]
public class RoomController : MonoBehaviour
{
    [SerializeField] private int size = 5;
    [SerializeField] private int wallHeight = 5;
    [SerializeField] private List<FurnitureData> startItems = new List<FurnitureData>
    {
        new FurnitureData("Chair", new Vector2Int(1, 1), false, 1f, new Color(0.9f, 0.45f, 0.4f)),
        new FurnitureData("Bed", new Vector2Int(1, 2), false, 0.6f, new Color(0.45f, 0.6f, 0.9f)),
        new FurnitureData("Table", new Vector2Int(2, 2), false, 0.8f, new Color(0.6f, 0.45f, 0.3f)),
        new FurnitureData("Lamp", new Vector2Int(1, 1), false, 1.8f, new Color(0.98f, 0.84f, 0.3f)),
        new FurnitureData("Picture", new Vector2Int(1, 1), true, 0f, new Color(0.55f, 0.8f, 0.5f)),
        new FurnitureData("Shelf", new Vector2Int(2, 1), true, 0f, new Color(0.75f, 0.55f, 0.85f))
    };

    private RoomView roomView;
    private FurnitureView furnitureView;
    private RoomInput roomInput;
    private RoomHud roomHud;
    private RoomGrid grid;
    private Inventory inventory;
    private PlacedFurniture moving;

    private void Awake()
    {
        roomView = GetComponent<RoomView>();
        furnitureView = GetComponent<FurnitureView>();
        roomInput = GetComponent<RoomInput>();
        roomHud = GetComponent<RoomHud>();
    }

    private void OnEnable()
    {
        roomInput.Pressed += OnPressed;
        roomInput.Dragged += OnDragged;
        roomHud.ItemClicked += OnItemClicked;
        roomHud.RotateClicked += OnRotateClicked;
        roomHud.PlaceClicked += OnPlaceClicked;
        roomHud.StoreClicked += OnStoreClicked;
    }

    private void OnDisable()
    {
        roomInput.Pressed -= OnPressed;
        roomInput.Dragged -= OnDragged;
        roomHud.ItemClicked -= OnItemClicked;
        roomHud.RotateClicked -= OnRotateClicked;
        roomHud.PlaceClicked -= OnPlaceClicked;
        roomHud.StoreClicked -= OnStoreClicked;
    }

    private void Start()
    {
        grid = new RoomGrid(size, wallHeight);
        inventory = new Inventory();
        foreach (FurnitureData item in startItems)
        {
            inventory.Add(item);
        }

        roomView.Show(grid);
        furnitureView.Refresh(grid.GetPlaced());
        roomHud.ShowInventory(inventory.GetItems());
    }

    private void OnItemClicked(int index)
    {
        if (moving != null)
        {
            return;
        }

        FurnitureData data = inventory.GetItems()[index];
        inventory.Remove(data);

        RoomSurface surface = data.isWallItem ? RoomSurface.RightWall : RoomSurface.Floor;
        Vector2Int center = new Vector2Int(
            (grid.GetWidth(surface) - data.size.x) / 2,
            (grid.GetHeight(surface) - data.size.y) / 2);

        BeginPlacing(new PlacedFurniture(data, surface, center));
    }

    private void OnPressed(Vector3 world)
    {
        if (moving == null)
        {
            TryPickUp(world);
            return;
        }

        MoveTo(world);
    }

    private void OnDragged(Vector3 world)
    {
        if (moving != null)
        {
            MoveTo(world);
        }
    }

    private void OnRotateClicked()
    {
        if (moving == null)
        {
            return;
        }

        moving.Rotate();
        moving.MoveTo(moving.GetSurface(), grid.ClampOrigin(moving.GetSurface(), moving.GetOrigin(), moving.GetSize()));
        UpdateGhost();
    }

    private void OnPlaceClicked()
    {
        if (moving != null && grid.Place(moving))
        {
            EndPlacing();
        }
    }

    private void OnStoreClicked()
    {
        if (moving == null)
        {
            return;
        }

        inventory.Add(moving.GetData());
        EndPlacing();
    }

    private void TryPickUp(Vector3 world)
    {
        if (!roomView.TryGetCell(world, out RoomSurface surface, out Vector2Int cell))
        {
            return;
        }

        PlacedFurniture furniture = grid.GetAt(surface, cell);
        if (furniture == null)
        {
            return;
        }

        grid.Remove(furniture);
        furnitureView.Refresh(grid.GetPlaced());
        BeginPlacing(furniture);
    }

    private void MoveTo(Vector3 world)
    {
        RoomSurface surface = moving.GetData().isWallItem ? roomView.GetNearestWall(world) : RoomSurface.Floor;
        Vector2Int footprint = moving.GetSize();
        Vector2Int cell = roomView.GetCellOn(surface, world);
        Vector2Int origin = cell - new Vector2Int((footprint.x - 1) / 2, (footprint.y - 1) / 2);

        moving.MoveTo(surface, grid.ClampOrigin(surface, origin, footprint));
        UpdateGhost();
    }

    private void BeginPlacing(PlacedFurniture furniture)
    {
        moving = furniture;
        roomView.SetGridVisible(true);
        roomHud.ShowPlacing(!furniture.GetData().isWallItem);
        UpdateGhost();
    }

    private void UpdateGhost()
    {
        bool isValid = grid.CanPlace(moving);
        furnitureView.ShowGhost(moving, isValid);
        roomHud.SetPlaceEnabled(isValid);
    }

    private void EndPlacing()
    {
        moving = null;
        furnitureView.HideGhost();
        roomView.SetGridVisible(false);
        furnitureView.Refresh(grid.GetPlaced());
        roomHud.ShowInventory(inventory.GetItems());
    }
}
