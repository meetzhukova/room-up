using System.Collections.Generic;
using UnityEngine;

public class PuzzleRound
{
    private const int CoinsPerItem = 2;
    private const int ComboBonusPerItem = 3;
    private const int ComboSize = 3;

    private Board board;
    private Wallet wallet;
    private Spawner spawner;
    private int roundCoins;
    private bool isOver;

    public PuzzleRound(int width, int height, Wallet wallet, Spawner spawner)
    {
        board = new Board(width, height);
        this.wallet = wallet;
        this.spawner = spawner;
    }

    public bool IsOver()
    {
        return isOver;
    }

    public bool Update(float deltaTime)
    {
        if (isOver || !spawner.Tick(deltaTime, board.GetFillRatio()))
        {
            return false;
        }

        if (spawner.SpawnBatch(board) == 0)
        {
            isOver = true;
            return false;
        }

        return true;
    }

    public int GetRoundCoins()
    {
        return roundCoins;
    }

    public Board GetBoard()
    {
        return board;
    }

    public void StartRound(float startFill)
    {
        int itemCount = Mathf.RoundToInt(board.GetWidth() * board.GetHeight() * startFill);
        int typeCount = System.Enum.GetValues(typeof(ItemType)).Length;

        for (int i = 0; i < itemCount; i++)
        {
            var emptyCells = board.GetEmptyCells();
            if (emptyCells.Count == 0)
            {
                return;
            }

            Vector2Int cell = emptyCells[Random.Range(0, emptyCells.Count)];
            ItemType type = (ItemType)Random.Range(0, typeCount);
            board.PlaceItem(new Item(type), cell);
        }
    }

    public TapResult Tap(Vector2Int cell)
    {
        List<Vector2Int> matched = new List<Vector2Int>();

        if (isOver || !board.IsEmpty(cell))
        {
            return new TapResult(matched, 0, false);
        }

        List<Vector2Int> found = board.FindNearestInCross(cell);

        foreach (Vector2Int candidate in found)
        {
            if (CountSameType(found, board.GetItem(candidate)) >= 2)
            {
                matched.Add(candidate);
            }
        }

        foreach (Vector2Int matchedCell in matched)
        {
            board.RemoveItem(matchedCell);
        }

        int reward = CalculateReward(matched.Count);
        roundCoins += reward;
        wallet.AddCoins(reward);

        return new TapResult(matched, reward, matched.Count >= ComboSize);
    }

    private int CalculateReward(int matchedCount)
    {
        int reward = matchedCount * CoinsPerItem;

        if (matchedCount >= ComboSize)
        {
            reward += (matchedCount - 2) * ComboBonusPerItem;
        }

        return reward;
    }

    private int CountSameType(List<Vector2Int> cells, Item item)
    {
        int count = 0;

        foreach (Vector2Int cell in cells)
        {
            if (board.GetItem(cell).IsSameType(item))
            {
                count++;
            }
        }

        return count;
    }
}
