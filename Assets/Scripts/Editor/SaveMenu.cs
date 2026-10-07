using UnityEditor;
using UnityEngine;

public static class SaveMenu
{
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
}
