using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureCatalog", menuName = "Room Up/Furniture Catalog")]
public class FurnitureCatalog : ScriptableObject
{
    [SerializeField] private List<FurnitureData> items = new List<FurnitureData>
    {
        new FurnitureData("Chair", new Vector2Int(1, 1), false, 1f, 20, new Color(0.9f, 0.45f, 0.4f)),
        new FurnitureData("Plant", new Vector2Int(1, 1), false, 1.4f, 25, new Color(0.4f, 0.75f, 0.45f)),
        new FurnitureData("Lamp", new Vector2Int(1, 1), false, 1.8f, 30, new Color(0.98f, 0.84f, 0.3f)),
        new FurnitureData("Picture", new Vector2Int(1, 1), true, 0f, 35, new Color(0.55f, 0.8f, 0.9f)),
        new FurnitureData("Shelf", new Vector2Int(2, 1), true, 0f, 50, new Color(0.75f, 0.55f, 0.85f)),
        new FurnitureData("Table", new Vector2Int(2, 2), false, 0.8f, 80, new Color(0.6f, 0.45f, 0.3f)),
        new FurnitureData("Bed", new Vector2Int(1, 2), false, 0.6f, 120, new Color(0.45f, 0.6f, 0.9f)),
        new FurnitureData("Sofa", new Vector2Int(1, 2), false, 0.9f, 150, new Color(0.9f, 0.55f, 0.7f))
    };

    public IReadOnlyList<FurnitureData> GetItems()
    {
        return items;
    }

    public FurnitureData Find(string itemName)
    {
        foreach (FurnitureData item in items)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }

        return null;
    }
}
