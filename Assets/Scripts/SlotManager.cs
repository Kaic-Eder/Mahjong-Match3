using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [Header("Referências da barra")]
    [SerializeField] private Transform[] slotPositions;

    [Header("Regras da barra")]
    [Range(1, 8)]
    [SerializeField] private int maxSlots = 7;

    private readonly List<TileController> currentTiles = new List<TileController>();

    public IReadOnlyList<TileController> CurrentTiles => currentTiles;
    public int MaxSlots => maxSlots;
    
    public bool TryAddTile(TileController tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("Tentativa de adicionar um tile nulo.");
            return false;
        }

        if (currentTiles.Count >= maxSlots)
        {
            Debug.LogWarning("A barra de slots está cheia.");
            return false;
        }

        if (currentTiles.Contains(tile))
        {
            Debug.LogWarning("Esse tile já está na barra.");
            return false;
        }

        int insertIndex = GetInsertionIndex(tile.TileTypeId);
        currentTiles.Insert(insertIndex, tile);

        Debug.Log($"[Tray] Tile inserido no índice {insertIndex}.");
        return true;
    }

    /// <summary>
    /// Encontra a posição correta na lista para agrupar tiles de mesmo tipo.
    /// </summary>
    private int GetInsertionIndex(int typeId)
    {
        for (int i = currentTiles.Count - 1; i >= 0; i--)
        {
            if (currentTiles[i].TileTypeId == typeId)
                return i + 1;
        }

        return currentTiles.Count;
    }
    
    public void RemoveTiles(IReadOnlyList<TileController> tilesToRemove)
    {
        if (tilesToRemove == null)
            return;

        foreach (TileController tile in tilesToRemove)
        {
            if (tile != null)
                currentTiles.Remove(tile);
        }

        Debug.Log($"[Tray] {tilesToRemove.Count} tile(s) removido(s) da lista.");
    }
    
    public bool TryGetSlotPosition(int index, out Vector3 position)
    {
        if (index >= 0 && index < slotPositions.Length && slotPositions[index] != null)
        {
            position = slotPositions[index].position;
            return true;
        }

        position = Vector3.zero;
        Debug.LogError($"Não existe posição configurada para o slot {index}.");
        return false;
    }
    
    
}
