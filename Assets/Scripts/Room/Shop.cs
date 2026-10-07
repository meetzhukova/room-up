using System.Collections.Generic;

public class Shop
{
    private readonly List<FurnitureData> catalog;

    public Shop(IEnumerable<FurnitureData> items)
    {
        catalog = new List<FurnitureData>(items);
    }

    public IReadOnlyList<FurnitureData> GetItems()
    {
        return catalog;
    }

    public bool CanBuy(FurnitureData item, Wallet wallet)
    {
        return catalog.Contains(item) && wallet.GetCoins() >= item.price;
    }

    public bool TryBuy(FurnitureData item, Wallet wallet, Inventory inventory)
    {
        if (!catalog.Contains(item) || !wallet.TrySpend(item.price))
        {
            return false;
        }

        inventory.Add(item);
        return true;
    }
}
