using System;
using System.Collections.Generic;

/// <summary>
/// Procura uma sequência de trios legal para um layout.
/// </summary>
public sealed class BoardSolver
{
    private readonly Random random;
    private readonly int maxCandidatesPerStep;

    public BoardSolver(
        int seed,
        int maxCandidatesPerStep = 300)
    {
        random = new Random(seed);
        this.maxCandidatesPerStep = maxCandidatesPerStep;
    }

    public bool TryCreateSolution(
        BoardLayout layout,
        out List<PlannedMatch> solution)
    {
        solution = new List<PlannedMatch>();

        if (layout.Count % 3 != 0)
        {
            solution = null;
            return false;
        }

        HashSet<int> removedIndexes = new HashSet<int>();

        bool solved = TryBuildSolution(
            layout,
            removedIndexes,
            solution);

        if (!solved)
        {
            solution = null;
            return false;
        }

        return true;
    }

    private bool TryBuildSolution(
        BoardLayout layout,
        HashSet<int> removedIndexes,
        List<PlannedMatch> solution)
    {
        if (removedIndexes.Count == layout.Count)
            return true;

        List<int> available =
            layout.GetAvailableIndexes(removedIndexes);

        if (available.Count < 3)
            return false;

        List<List<int>> candidates =
            CreateCandidateTriples(available);

        Shuffle(candidates);

        int candidateCount = Math.Min(
            candidates.Count,
            maxCandidatesPerStep);

        for (int i = 0; i < candidateCount; i++)
        {
            List<int> candidate = candidates[i];

            foreach (int cellIndex in candidate)
                removedIndexes.Add(cellIndex);

            PlannedMatch plannedMatch = new PlannedMatch(
                solution.Count,
                candidate);

            solution.Add(plannedMatch);

            bool solved = TryBuildSolution(
                layout,
                removedIndexes,
                solution);

            if (solved)
                return true;

            solution.RemoveAt(solution.Count - 1);

            foreach (int cellIndex in candidate)
                removedIndexes.Remove(cellIndex);
        }

        return false;
    }

    private List<List<int>> CreateCandidateTriples(
        List<int> available)
    {
        List<List<int>> candidates =
            new List<List<int>>();

        for (int a = 0; a < available.Count - 2; a++)
        {
            for (int b = a + 1; b < available.Count - 1; b++)
            {
                for (int c = b + 1; c < available.Count; c++)
                {
                    candidates.Add(new List<int>
                    {
                        available[a],
                        available[b],
                        available[c]
                    });
                }
            }
        }

        return candidates;
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