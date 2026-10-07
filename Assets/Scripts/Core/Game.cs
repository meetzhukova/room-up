using UnityEngine;
using UnityEngine.SceneManagement;

public static class Game
{
    public const string RoomScene = "Room";
    public const string PuzzleScene = "Puzzle";

    private const string CatalogPath = "FurnitureCatalog";

    private static PlayerProgress progress;
    private static FurnitureCatalog catalog;

    public static FurnitureCatalog Catalog
    {
        get
        {
            if (catalog == null)
            {
                catalog = Resources.Load<FurnitureCatalog>(CatalogPath);
                if (catalog == null)
                {
                    Debug.LogError("FurnitureCatalog not found. Create it in Assets/Resources.");
                }
            }
            return catalog;
        }
    }

    public static PlayerProgress Progress
    {
        get
        {
            if (progress == null)
            {
                SaveData data = SaveSystem.Load();
                progress = data == null ? new PlayerProgress() : PlayerProgress.FromSaveData(data, Catalog);
            }
            return progress;
        }
    }

    public static void Save()
    {
        if (progress != null)
        {
            SaveSystem.Save(progress.ToSaveData());
        }
    }

    public static void OpenRoom()
    {
        Save();
        SceneManager.LoadScene(RoomScene);
    }

    public static void OpenPuzzle()
    {
        Save();
        SceneManager.LoadScene(PuzzleScene);
    }
}
