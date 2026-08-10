using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [Header("---Slots Configurations---")]

    [Tooltip("Arraste aqui os GameObjects vazios que representam as posições dos slots na UI/Cena")]
    [SerializeField] private Transform[] slotPositions;
    
    [Tooltip("Capacidade máxima da barra de slots")]
    [Range(1, 8)]
    [SerializeField] private int maxSlots = 7;
    
    private List<TileController> currentTiles = new List<TileController>();
    
    /// <summary>
    /// Tenta adicionar um tile selecionado à barra de slots.
    /// </summary>
    public bool TryAddTile(TileController tile)
    {
        // 1. Verifica se a barra já está cheia
        if (currentTiles.Count >= maxSlots)
        {
            Debug.LogWarning( "A barra de slots está cheia!");
            Debug.LogWarning( "Parece que voce perdeu");
            return false;
        }

        int insertIndex = GetInsertIndex(tile.TileTypeId);
        
        currentTiles.Insert(insertIndex, tile);

        StartCoroutine(ProcessarJogadaRoutine(tile.TileTypeId));
        
        return true;
    }
    
    private IEnumerator ProcessarJogadaRoutine(int typeId)
    {
        // PASSO 1: Atualiza e ESPERA a animação de movimento terminar!
        yield return StartCoroutine(UpdateTilePositionRoutine());

        // PASSO 2: Agora que os tiles pousaram suavemente no slot, checamos o Match!
        CheckForMatch(typeId);
    }

    private int GetInsertIndex(int typeId)
    {
        for (int i = currentTiles.Count - 1; i >= 0; i--)
        {
            if (currentTiles[i].TileTypeId == typeId)
            {
                return i + 1;
            }
        }
        
        return currentTiles.Count;
        
    }

    private IEnumerator UpdateTilePositionRoutine()
    {
        Coroutine ultimaAnimacao = null;
        for (int i = 0; i < currentTiles.Count; i++)
        {
            Vector3 destino = slotPositions[i].position;
            ultimaAnimacao = currentTiles[i].MoverPara(destino);
        }
        if (ultimaAnimacao != null)
        {
            yield return ultimaAnimacao;
        }
    }

    private void CheckForMatch(int typeId)
    {
        List<TileController> matchingTiles = new  List<TileController>();

        foreach (var t in currentTiles)
        {
            if (t.TileTypeId == typeId)
            {
                matchingTiles.Add(t);
            }
        }

        if (matchingTiles.Count >= 3)
        {
            Debug.Log($"<color=green>MATCH! 3 peças do tipo {typeId} combinadas!</color>");

            foreach (var tileToRemove in matchingTiles)
            {
                currentTiles.Remove(tileToRemove);
                
                Destroy(tileToRemove.gameObject);
            }
            StartCoroutine(UpdateTilePositionRoutine());
            
        }
        
    }
    
    
}
