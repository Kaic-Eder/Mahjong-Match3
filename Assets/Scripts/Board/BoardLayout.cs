using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contém a geometria completa de uma fase.
/// A classe não cria GameObjects.
/// </summary>
public sealed class BoardLayout
{
    private readonly List<BoardCell> cells;

    public IReadOnlyList<BoardCell> Cells => cells;
    public int Count => cells.Count;

    public BoardLayout(List<BoardCell> cells)
    {
        this.cells = cells;
    }

    /// <summary>
    /// Uma célula está bloqueada somente quando há uma célula ativa
    /// em camada superior na mesma coordenada X/Y.
    /// Não existe bloqueio lateral nesta regra.
    /// </summary>
    public bool IsBlocked(
        int candidateIndex,
        HashSet<int> removedIndexes)
    {
        BoardCell candidate = cells[candidateIndex];

        for (int i = 0; i < cells.Count; i++)
        {
            if (i == candidateIndex)
                continue;

            if (removedIndexes.Contains(i))
                continue;

            BoardCell possibleCover = cells[i];

            bool isAbove = possibleCover.Layer > candidate.Layer;
            bool sameColumn =
                possibleCover.GridPosition == candidate.GridPosition;

            if (isAbove && sameColumn)
                return true;
        }

        return false;
    }

    public List<int> GetAvailableIndexes(HashSet<int> removedIndexes)
    {
        List<int> available = new List<int>();

        for (int i = 0; i < cells.Count; i++)
        {
            if (removedIndexes.Contains(i))
                continue;

            if (!IsBlocked(i, removedIndexes))
                available.Add(i);
        }

        return available;
    }
}