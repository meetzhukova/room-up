using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int version = 1;
    public int coins;
    public int bestRoundCoins;
    public List<string> inventory = new List<string>();
    public List<PlacedItemSave> placed = new List<PlacedItemSave>();
}

[System.Serializable]
public class PlacedItemSave
{
    public string item;
    public RoomSurface surface;
    public Vector2Int origin;
    public int rotation;
}
