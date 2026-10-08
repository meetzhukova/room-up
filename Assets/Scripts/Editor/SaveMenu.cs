using UnityEditor;
using UnityEngine;

public static class SaveMenu
{
    private const int CoinsToAdd = 500;

    [MenuItem("Room Up/Delete Save")]
    private static void DeleteSave()
    {
        SaveSystem.Delete();
        Debug.Log("Save deleted: " + SaveSystem.GetPath());
    }

    [MenuItem("Room Up/Show Save File")]
    private static void ShowSaveFile()
    {
        EditorUtility.RevealInFinder(SaveSystem.GetPath());
    }

    [MenuItem("Room Up/Add 500 Coins")]
    private static void AddCoins()
    {
        if (Application.isPlaying)
        {
            Game.Progress.GetWallet().AddCoins(CoinsToAdd);
            Game.Save();
            Debug.Log("Added " + CoinsToAdd + " coins. Total: " + Game.Progress.GetWallet().GetCoins() + ". Switch scene to see it on screen.");
            return;
        }

        SaveData data = SaveSystem.Load() ?? new SaveData();
        data.coins += CoinsToAdd;
        SaveSystem.Save(data);
        Debug.Log("Added " + CoinsToAdd + " coins to the save file. Total: " + data.coins);
    }
}
