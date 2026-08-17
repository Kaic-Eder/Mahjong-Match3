using System.Collections.Generic;

/// <summary>
/// Um trio que o gerador sabe que pode ser removido
/// em determinado momento da solução.
/// </summary>
public sealed class PlannedMatch
{
    public int MatchNumber { get; }
    public int TileTypeId { get; set; }
    public List<int> CellIndexes { get; }

    public PlannedMatch(
        int matchNumber,
        List<int> cellIndexes)
    {
        MatchNumber = matchNumber;
        CellIndexes = cellIndexes;
        TileTypeId = -1;
    }
}