using UnityEngine.SceneManagement;

public static class Game
{
    public const string RoomScene = "Room";
    public const string PuzzleScene = "Puzzle";

    public static PlayerProgress Progress { get; } = new PlayerProgress();

    public static void OpenRoom()
    {
        SceneManager.LoadScene(RoomScene);
    }

    public static void OpenPuzzle()
    {
        SceneManager.LoadScene(PuzzleScene);
    }
}
