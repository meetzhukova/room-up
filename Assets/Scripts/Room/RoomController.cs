using UnityEngine;

[RequireComponent(typeof(RoomView), typeof(FurnitureView), typeof(RoomInput))]
[RequireComponent(typeof(RoomHud), typeof(ShopView))]
[DisallowMultipleComponent]
public class RoomController : MonoBehaviour
{
    [SerializeField] private int size = 5;
    [SerializeField] private int wallHeight = 5;
    [SerializeField] private int startCoins = 0;

    private RoomView roomView;
    private FurnitureView furnitureView;
    private RoomInput roomInput;
    private RoomHud roomHud;
    private ShopView shopView;
    private Shop shop;
    private Wallet wallet;
    private RoomGrid grid;
    private Inventory inventory;
    private PlacedFurniture moving;

    private void Awake()
    {
        roomView = GetComponent<RoomView>();
        furnitureView = GetComponent<FurnitureView>();
        roomInput = GetComponent<RoomInput>();
        roomHud = GetComponent<RoomHud>();
        shopView = GetComponent<ShopView>();
    }

    private void OnEnable()
    {
        roomInput.Pressed += OnPressed;
        roomInput.Dragged += OnDragged;
        roomHud.ItemClicked += OnItemClicked;
        roomHud.RotateClicked += OnRotateClicked;
        roomHud.PlaceClicked += OnPlaceClicked;
        roomHud.StoreClicked += OnStoreClicked;
        roomHud.PlayClicked += OnPlayClicked;
        roomHud.ShopClicked += OnShopClicked;
        shopView.BuyClicked += OnBuyClicked;
        shopView.CloseClicked += OnCloseShopClicked;
    }

    private void OnDisable()
    {
        roomInput.Pressed -= OnPressed;
        roomInput.Dragged -= OnDragged;
        roomHud.ItemClicked -= OnItemClicked;
        roomHud.RotateClicked -= OnRotateClicked;
        roomHud.PlaceClicked -= OnPlaceClicked;
        roomHud.StoreClicked -= OnStoreClicked;
        roomHud.PlayClicked -= OnPlayClicked;
        roomHud.ShopClicked -= OnShopClicked;
        shopView.BuyClicked -= OnBuyClicked;
        shopView.CloseClicked -= OnCloseShopClicked;
    }

    private void Start()
    {
        PlayerProgress progress = Game.Progress;
        inventory = progress.GetInventory();
        wallet = progress.GetWallet();
        shop = new Shop(Game.Catalog.GetItems());

        if (progress.IsNew())
        {
            wallet.AddCoins(startCoins);
            progress.MarkStarted();
        }

        grid = new RoomGrid(size, wallHeight);
        foreach (PlacedFurniture furniture in progress.GetPlaced())
        {
            if (!grid.Place(furniture))
            {
                inventory.Add(furniture.GetData());
            }
        }
        progress.SetPlaced(grid.GetPlaced());

        roomView.Show(grid);
        roomHud.SetCoins(wallet.GetCoins());
        furnitureView.Refresh(grid.GetPlaced());
        roomHud.ShowInventory(inventory.GetItems());
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            SaveWithMovingItem();
        }
    }

    private void OnApplicationQuit()
    {
        SaveWithMovingItem();
    }

    private void SaveWithMovingItem()
    {
        if (moving != null)
        {
            inventory.Add(moving.GetData());
            Game.Save();
            inventory.Remove(moving.GetData());
            return;
        }

        Game.Save();
    }

    private void OnPlayClicked()
    {
        if (moving != null)
        {
            inventory.Add(moving.GetData());
            moving = null;
        }

        Game.OpenPuzzle();
    }

    private void OnShopClicked()
    {
        if (moving != null)
        {
            return;
        }

        shopView.Show(shop.GetItems(), wallet.GetCoins());
    }

    private void OnBuyClicked(int index)
    {
        FurnitureData item = shop.GetItems()[index];
        if (!shop.TryBuy(item, wallet, inventory))
        {
            return;
        }

        roomHud.SetCoins(wallet.GetCoins());
        roomHud.ShowInventory(inventory.GetItems());
        shopView.Show(shop.GetItems(), wallet.GetCoins());
        Game.Save();
    }

    private void OnCloseShopClicked()
    {
        shopView.Hide();
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
        Game.Progress.SetPlaced(grid.GetPlaced());
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
        Game.Progress.SetPlaced(grid.GetPlaced());
        Game.Save();
        furnitureView.HideGhost();
        roomView.SetGridVisible(false);
        furnitureView.Refresh(grid.GetPlaced());
        roomHud.ShowInventory(inventory.GetItems());
    }
}
