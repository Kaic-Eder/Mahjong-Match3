using System;
using System.Collections.Generic;

/// <summary>
/// Atribui tipos de tile aos grupos produzidos pelo solver.
/// </summary>
public sealed class BoardContentGenerator
{
    private readonly Random random;

    public BoardContentGenerator(int seed)
    {
        random = new Random(seed);
    }

    public int[] AssignUniqueTypes(
        int cellCount,
        List<PlannedMatch> solution,
        int availableTypeCount)
    {
        if (availableTypeCount < solution.Count)
        {
            throw new InvalidOperationException(
                "A quantidade de sprites disponíveis é menor que " +
                "a quantidade de trios da fase.");
        }

        int[] typesByCell = new int[cellCount];

        for (int i = 0; i < typesByCell.Length; i++)
            typesByCell[i] = -1;

        List<int> typeIds = new List<int>();

        for (int i = 0; i < availableTypeCount; i++)
            typeIds.Add(i);

        Shuffle(typeIds);

        for (int matchIndex = 0;
             matchIndex < solution.Count;
             matchIndex++)
        {
            PlannedMatch match = solution[matchIndex];
            int typeId = typeIds[matchIndex];
            match.TileTypeId = typeId;

            foreach (int cellIndex in match.CellIndexes)
                typesByCell[cellIndex] = typeId;
        }

        return typesByCell;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            T temporary = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temporary;
        }
    }
}