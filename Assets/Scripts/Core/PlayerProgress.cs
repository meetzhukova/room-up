using System.Collections.Generic;

public class PlayerProgress
{
    private readonly Wallet wallet = new Wallet();
    private readonly Inventory inventory = new Inventory();
    private readonly List<PlacedFurniture> placed = new List<PlacedFurniture>();
    private bool isNew = true;

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

    public bool IsNew()
    {
        return isNew;
    }

    public void MarkStarted()
    {
        isNew = false;
    }
}
