using UnityEngine;

/// <summary>
/// Representa uma posição lógica do tabuleiro.
/// Não é um GameObject e não possui comportamento de cena.
/// </summary>
public readonly struct BoardCell
{
    public int Index { get; }
    public Vector2Int GridPosition { get; }
    public int Layer { get; }
    public Vector3 WorldPosition { get; }

    public BoardCell(
        int index,
        Vector2Int gridPosition,
        int layer,
        Vector3 worldPosition)
    {
        Index = index;
        GridPosition = gridPosition;
        Layer = layer;
        WorldPosition = worldPosition;
    }
}