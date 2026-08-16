using System.Collections.Generic;

public class MatchFinder
{
    public bool TryFindMatch(
        IReadOnlyList<TileController> tiles,
        int typeId,
        int requiredAmount,
        out List<TileController> match)
    {
        match = new List<TileController>();

        foreach (TileController tile in tiles)
        {
            if (tile == null || tile.TileTypeId != typeId)
                continue;

            match.Add(tile);

            if (match.Count == requiredAmount)
                return true;
        }

        match.Clear();
        return false;
    }
}