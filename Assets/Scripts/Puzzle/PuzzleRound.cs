using System.Collections.Generic;
using UnityEngine;

public class PuzzleRound
{
    private Board board;

    public PuzzleRound(int width, int height)
    {
        board = new Board(width, height);
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

    public List<Vector2Int> Tap(Vector2Int cell)
    {
        List<Vector2Int> matched = new List<Vector2Int>();

        if (!board.IsEmpty(cell))
        {
            return matched;
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

        return matched;
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
