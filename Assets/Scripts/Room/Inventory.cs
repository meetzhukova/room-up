using System.Collections.Generic;

public class Inventory
{
    private readonly List<FurnitureData> items = new List<FurnitureData>();

    public IReadOnlyList<FurnitureData> GetItems()
    {
        return items;
    }

    public void Add(FurnitureData item)
    {
        items.Add(item);
    }

    public bool Remove(FurnitureData item)
    {
        return items.Remove(item);
    }
}
