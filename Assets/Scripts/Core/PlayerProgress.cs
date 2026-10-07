using System.Collections.Generic;

public class PlayerProgress
{
    private readonly Wallet wallet;
    private readonly Inventory inventory = new Inventory();
    private readonly List<PlacedFurniture> placed = new List<PlacedFurniture>();
    private int bestRoundCoins;
    private bool isNew = true;

    public PlayerProgress()
    {
        wallet = new Wallet();
    }

    private PlayerProgress(int coins)
    {
        wallet = new Wallet(coins);
    }

    public Wallet GetWallet()
    {
        return wallet;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public IReadOnlyList<PlacedFurniture> GetPlaced()
    {
        return placed;
    }

    public void SetPlaced(IEnumerable<PlacedFurniture> items)
    {
        placed.Clear();
        placed.AddRange(items);
    }

    public int GetBestRoundCoins()
    {
        return bestRoundCoins;
    }

    public bool TrySetBestRoundCoins(int roundCoins)
    {
        if (roundCoins <= bestRoundCoins)
        {
            return false;
        }

        bestRoundCoins = roundCoins;
        return true;
    }

    public bool IsNew()
    {
        return isNew;
    }

    public void MarkStarted()
    {
        isNew = false;
    }

    public SaveData ToSaveData()
    {
        SaveData data = new SaveData
        {
            coins = wallet.GetCoins(),
            bestRoundCoins = bestRoundCoins
        };

        foreach (FurnitureData item in inventory.GetItems())
        {
            data.inventory.Add(item.name);
        }

        foreach (PlacedFurniture furniture in placed)
        {
            data.placed.Add(new PlacedItemSave
            {
                item = furniture.GetData().name,
                surface = furniture.GetSurface(),
                origin = furniture.GetOrigin(),
                rotation = furniture.GetRotation()
            });
        }

        return data;
    }

    public static PlayerProgress FromSaveData(SaveData data, FurnitureCatalog catalog)
    {
        PlayerProgress progress = new PlayerProgress(data.coins);
        progress.bestRoundCoins = data.bestRoundCoins;
        progress.isNew = false;

        if (catalog == null)
        {
            return progress;
        }

        foreach (string itemName in data.inventory)
        {
            FurnitureData item = catalog.Find(itemName);
            if (item != null)
            {
                progress.inventory.Add(item);
            }
        }

        foreach (PlacedItemSave saved in data.placed)
        {
            FurnitureData item = catalog.Find(saved.item);
            if (item != null)
            {
                progress.placed.Add(new PlacedFurniture(item, saved.surface, saved.origin, saved.rotation));
            }
        }

        return progress;
    }
}
