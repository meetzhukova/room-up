using System.Collections.Generic;
using UnityEngine;

public class TapResult
{
    public List<Vector2Int> MatchedCells { get; }
    public int Reward { get; }
    public bool IsCombo { get; }

    public TapResult(List<Vector2Int> matchedCells, int reward, bool isCombo)
    {
        MatchedCells = matchedCells;
        Reward = reward;
        IsCombo = isCombo;
    }
}
