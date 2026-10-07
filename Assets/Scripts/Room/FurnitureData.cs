using UnityEngine;

[System.Serializable]
public class FurnitureData
{
    public string name;
    public Vector2Int size = Vector2Int.one;
    public bool isWallItem;
    public float height = 1f;
    public Color color = Color.white;

    public FurnitureData()
    {
    }

    public FurnitureData(string name, Vector2Int size, bool isWallItem, float height, Color color)
    {
        this.name = name;
        this.size = size;
        this.isWallItem = isWallItem;
        this.height = height;
        this.color = color;
    }
}
