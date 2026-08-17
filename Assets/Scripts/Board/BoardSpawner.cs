using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Converte dados do tabuleiro em GameObjects.
/// </summary>
public sealed class BoardSpawner : MonoBehaviour
{
    [Header("Prefab e raiz")]
    [SerializeField] private TileController tilePrefab;
    [SerializeField] private Transform boardRoot;

    [Header("Sprites")]
    [Tooltip("O índice do array precisa corresponder ao TileTypeId.")]
    [SerializeField] private Sprite[] tileSprites;

    public List<TileController> Spawn(
        BoardLayout layout,
        int[] typesByCell)
    {
        if (tilePrefab == null)
        {
            Debug.LogError("BoardSpawner: tilePrefab não foi configurado.");
            return new List<TileController>();
        }

        if (boardRoot == null)
            boardRoot = transform;

        if (typesByCell.Length != layout.Count)
        {
            Debug.LogError(
                "BoardSpawner: quantidade de tipos diferente " +
                "da quantidade de células.");

            return new List<TileController>();
        }

        List<TileController> spawned =
            new List<TileController>();

        for (int i = 0; i < layout.Count; i++)
        {
            BoardCell cell = layout.Cells[i];
            int typeId = typesByCell[i];

            if (typeId < 0 || typeId >= tileSprites.Length)
            {
                Debug.LogError(
                    $"BoardSpawner: typeId {typeId} não possui sprite.");

                continue;
            }

            TileController tile = Instantiate(
                tilePrefab,
                cell.WorldPosition,
                Quaternion.identity,
                boardRoot);

            tile.name =
                $"BoardTile_{i}_Type_{typeId}_Layer_{cell.Layer}";

            tile.Initialize(
                typeId,
                tileSprites[typeId],
                cell.Index,
                cell.Layer);

            spawned.Add(tile);
        }

        return spawned;
    }

    public void ClearBoard()
    {
        if (boardRoot == null)
            boardRoot = transform;

        for (int i = boardRoot.childCount - 1; i >= 0; i--)
        {
            GameObject child = boardRoot.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }
}