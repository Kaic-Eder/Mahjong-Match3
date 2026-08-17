using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coordena a criação e o estado das peças do tabuleiro.
/// </summary>
public sealed class BoardController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private BoardSpawner spawner;

    [Header("Layout")]
    [SerializeField] private BoardShape boardShape = BoardShape.Flower;
    [SerializeField] private float cellSpacing = 0.95f;
    [SerializeField] private float layerDepth = 0.10f;
    [SerializeField] private Vector3 boardOrigin;

    [Header("Geração")]
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private int seed = 12345;
    [SerializeField] private int generationAttempts = 30;

    private BoardLayout currentLayout;
    private readonly Dictionary<int, TileController> activeTiles =
        new Dictionary<int, TileController>();

    private HashSet<int> removedIndexes =
        new HashSet<int>();

    public BoardLayout CurrentLayout => currentLayout;

    private void Start()
    {
        if (generateOnStart)
            GenerateBoard();
    }

    [ContextMenu("Generate Board")]
    public void GenerateBoard()
    {
        if (spawner == null)
        {
            Debug.LogError("BoardController: spawner não foi configurado.");
            return;
        }

        spawner.ClearBoard();
        activeTiles.Clear();
        removedIndexes.Clear();

        int baseSeed = seed;
        bool solutionFound = false;
        List<PlannedMatch> solution = null;

        for (int attempt = 0;
             attempt < generationAttempts;
             attempt++)
        {
            int attemptSeed = baseSeed + attempt;
            BoardLayoutGenerator layoutGenerator =
                new BoardLayoutGenerator(
                    cellSpacing,
                    layerDepth,
                    boardOrigin);

            BoardLayout candidateLayout =
                CreateLayout(layoutGenerator);

            if (candidateLayout.Count % 3 != 0)
            {
                Debug.LogError(
                    $"O layout possui {candidateLayout.Count} células. " +
                    "A quantidade precisa ser múltipla de três.");

                return;
            }

            BoardSolver solver = new BoardSolver(attemptSeed);

            if (solver.TryCreateSolution(
                    candidateLayout,
                    out List<PlannedMatch> candidateSolution))
            {
                currentLayout = candidateLayout;
                solution = candidateSolution;
                solutionFound = true;
                break;
            }
        }

        if (!solutionFound)
        {
            Debug.LogError(
                "Não foi possível criar uma solução para o layout " +
                $"após {generationAttempts} tentativas.");

            return;
        }

        BoardContentGenerator contentGenerator =
            new BoardContentGenerator(baseSeed);

        int[] typesByCell =
            contentGenerator.AssignUniqueTypes(
                currentLayout.Count,
                solution,
                availableTypeCount: 15);

        List<TileController> spawned =
            spawner.Spawn(currentLayout, typesByCell);

        foreach (TileController tile in spawned)
        {
            if (tile == null)
                continue;

            activeTiles[tile.BoardCellIndex] = tile;
        }

        RefreshAccessibility();
        PrintSolution(solution);

        Debug.Log(
            $"[Board] Gerado com {currentLayout.Count} tiles e " +
            $"{solution.Count} trios.");
    }

    /// <summary>
    /// Chamado pelo GameFlowController quando o jogador seleciona
    /// uma peça do tabuleiro.
    /// </summary>
    public void NotifyTileSelected(TileController tile)
    {
        if (tile == null)
            return;

        int cellIndex = tile.BoardCellIndex;

        if (cellIndex < 0)
            return;

        if (!activeTiles.Remove(cellIndex))
            return;

        removedIndexes.Add(cellIndex);
        RefreshAccessibility();
    }

    private void RefreshAccessibility()
    {
        foreach (KeyValuePair<int, TileController> pair
                 in activeTiles)
        {
            int cellIndex = pair.Key;
            TileController tile = pair.Value;

            if (tile == null)
                continue;

            bool blocked = currentLayout.IsBlocked(
                cellIndex,
                removedIndexes);

            tile.SetInteractable(!blocked);
        }
    }

    private BoardLayout CreateLayout(
        BoardLayoutGenerator generator)
    {
        switch (boardShape)
        {
            case BoardShape.Cross:
                return generator.CreateCross();

            case BoardShape.Diamond:
                return generator.CreateDiamond();

            case BoardShape.Flower:
            default:
                return generator.CreateFlower();
        }
    }

    private void PrintSolution(
        List<PlannedMatch> solution)
    {
        for (int i = 0; i < solution.Count; i++)
        {
            PlannedMatch match = solution[i];
            string indexes = string.Join(", ", match.CellIndexes);

            Debug.Log(
                $"[Board] Solução {i}: " +
                $"tipo {match.TileTypeId}, células [{indexes}]");
        }
    }
}

public enum BoardShape
{
    Flower,
    Cross,
    Diamond
}