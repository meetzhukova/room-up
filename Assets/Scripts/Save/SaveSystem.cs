using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string FileName = "save.json";

    public static string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, FileName);
    }

    public static void Save(SaveData data)
    {
        File.WriteAllText(GetPath(), JsonUtility.ToJson(data, true));
    }

    public static SaveData Load()
    {
        string path = GetPath();
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Save file could not be read: " + exception.Message);
            return null;
        }
    }

    public static void Delete()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
