using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [Header("---Slots Configurations---")]

    [Tooltip("Arraste aqui os GameObjects vazios que representam as posições dos slots na UI/Cena")]
    [SerializeField] private Transform[] slotPositions;
    
    [Tooltip("Capacidade máxima da barra de slots")]
    [Range(1, 8)]
    [SerializeField] private int maxSlots = 7;
    [SerializeField] private float velocidadeMovimento = 12f;
    private List<TileController> currentTiles = new List<TileController>();
    
    /// <summary>
    /// Tenta adicionar e organizar um Tile na barra de slots.
    /// </summary>
    public bool TryAddTile(TileController tile)
    {
        if (currentTiles.Count >= maxSlots)
        {
            Debug.LogWarning("A barra de slots está cheia! Game Over?");
            return false;
        }

        // 1. Descobre em qual índice o tile deve ser inserido
        int insertIndex = GetInsertionIndex(tile.TileTypeId);

        // 2. Insere o tile na lista na posição correta
        currentTiles.Insert(insertIndex, tile);

        // 3. Atualiza a posição visual de TODOS os tiles na barra
        UpdateTilePositions();

        // 4. Verifica se essa inserção gerou um Match de 3 peças
        CheckForMatch(tile.TileTypeId);

        return true;
    }

    /// <summary>
    /// Encontra a posição correta na lista para agrupar tiles de mesmo tipo.
    /// </summary>
    private int GetInsertionIndex(int typeId)
    {
        // Procura do final para o começo pelo último tile do mesmo tipo
        for (int i = currentTiles.Count - 1; i >= 0; i--)
        {
            if (currentTiles[i].TileTypeId == typeId)
            {
                return i + 1; // Insere logo após o último encontrado
            }
        }

        // Se não encontrou nenhum igual, entra no final da fila
        return currentTiles.Count;
    }

    private void UpdateTilePositions()
    {
        for (int i = 0; i < currentTiles.Count; i++)
        {
            // Move instantaneamente para o slot correto (depois faremos a animação)
            float duration = Vector3.Distance(currentTiles[i].transform.position, slotPositions[i].position)/velocidadeMovimento;
            currentTiles[i].transform.DOMove(slotPositions[i].transform.position, duration).SetEase(Ease.Linear);
        }
    }

    /// <summary>
    /// Verifica se há 3 peças do mesmo tipo e processa o Match.
    /// </summary>
    private void CheckForMatch(int typeId)
    {
        // Lista temporária para guardar os tiles correspondentes ao match
        List<TileController> matchingTiles = new List<TileController>();

        foreach (var t in currentTiles)
        {
            if (t.TileTypeId == typeId)
            {
                matchingTiles.Add(t);
            }
        }

        // Se encontramos 3 peças do mesmo tipo!
        if (matchingTiles.Count >= 3)
        {
            Debug.Log($"<color=green>MATCH! 3 peças do tipo {typeId} combinadas!</color>");

            // 1. Remove os 3 tiles da nossa lista lógica
            foreach (var tileToRemove in matchingTiles)
            {
                currentTiles.Remove(tileToRemove);
                
                // 2. Destrói o GameObject da cena
                Destroy(tileToRemove.gameObject);
            }

            // 3. Reorganiza os tiles que sobraram na barra
            UpdateTilePositions();
        }
    }
    
    
}
