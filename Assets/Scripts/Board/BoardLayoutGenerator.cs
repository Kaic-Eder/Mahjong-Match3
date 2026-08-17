using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cria somente a forma do tabuleiro.
/// Não escolhe sprites e não instancia prefabs.
/// </summary>
public sealed class BoardLayoutGenerator
{
    private readonly float cellSpacing;
    private readonly float layerDepth;
    private readonly Vector3 origin;

    public BoardLayoutGenerator(
        float cellSpacing,
        float layerDepth,
        Vector3 origin)
    {
        this.cellSpacing = cellSpacing;
        this.layerDepth = layerDepth;
        this.origin = origin;
    }

    /// <summary>
    /// Cria uma forma com 45 células:
    /// camada 0 = 6 x 5 = 30
    /// camada 1 = 4 x 3 = 12
    /// camada 2 = 3 x 1 = 3
    /// total = 45 = 15 trios
    /// </summary>
    public BoardLayout CreateFlower()
    {
        List<BoardCell> cells = new List<BoardCell>();

        AddRectangle(
            cells,
            width: 6,
            height: 5,
            layer: 0);

        AddRectangle(
            cells,
            width: 4,
            height: 3,
            layer: 1);

        AddRectangle(
            cells,
            width: 3,
            height: 1,
            layer: 2);

        return new BoardLayout(cells);
    }

    /// <summary>
    /// Cria uma forma em cruz com três camadas.
    /// O total é 27: 21 + 3 + 3.
    /// </summary>
    public BoardLayout CreateCross()
    {
        List<BoardCell> cells = new List<BoardCell>();

        AddRectangle(
            cells,
            width: 7,
            height: 3,
            layer: 0);

        AddRectangle(
            cells,
            width: 3,
            height: 3,
            layer: 1);

        AddRectangle(
            cells,
            width: 3,
            height: 1,
            layer: 2);

        return new BoardLayout(cells);
    }

    /// <summary>
    /// Cria uma forma de diamante aproximada usando máscaras.
    /// O resultado deve ser validado para confirmar que a quantidade
    /// é múltipla de três antes de ser usado em uma fase.
    /// </summary>
    public BoardLayout CreateDiamond()
    {
        List<BoardCell> cells = new List<BoardCell>();

        int[,] mask =
        {
            { 0, 0, 1, 0, 0 },
            { 0, 1, 1, 1, 0 },
            { 1, 1, 1, 1, 1 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 1, 0, 0 }
        };

        for (int y = 0; y < mask.GetLength(0); y++)
        {
            for (int x = 0; x < mask.GetLength(1); x++)
            {
                if (mask[y, x] == 0)
                    continue;

                AddCell(
                    cells,
                    x - 2,
                    y - 2,
                    layer: 0);
            }
        }

        // Pequeno topo central para criar profundidade.
        AddRectangle(
            cells,
            width: 3,
            height: 1,
            layer: 1);

        return new BoardLayout(cells);
    }

    private void AddRectangle(
        List<BoardCell> cells,
        int width,
        int height,
        int layer)
    {
        int offsetX = -(width / 2);
        int offsetY = -(height / 2);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                AddCell(
                    cells,
                    offsetX + x,
                    offsetY + y,
                    layer);
            }
        }
    }

    private void AddCell(
        List<BoardCell> cells,
        int gridX,
        int gridY,
        int layer)
    {
        int index = cells.Count;

        Vector3 worldPosition = origin + new Vector3(
            gridX * cellSpacing,
            gridY * cellSpacing,
            -layer * layerDepth);

        cells.Add(new BoardCell(
            index,
            new Vector2Int(gridX, gridY),
            layer,
            worldPosition));
    }
}