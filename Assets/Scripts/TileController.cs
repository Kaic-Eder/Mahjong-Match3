using DG.Tweening;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [Header("Identidade")]
    [SerializeField] private int tileTypeId;
    [SerializeField] private int boardCellIndex = -1;
    [SerializeField] private int boardLayer;

    [Header("Referências locais")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D tileCollider;

    public int TileTypeId => tileTypeId;
    public int BoardCellIndex => boardCellIndex;
    public int BoardLayer => boardLayer;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (tileCollider == null)
            tileCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Configura o tile depois que ele foi instanciado pelo BoardSpawner.
    /// O tipo deixa de depender do nome do arquivo do sprite.
    /// </summary>
    public void Initialize(
        int typeId,
        Sprite sprite,
        int cellIndex,
        int layer)
    {
        tileTypeId = typeId;
        boardCellIndex = cellIndex;
        boardLayer = layer;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = layer * 10;
        }
    }

    public void SetInteractable(bool value)
    {
        if (tileCollider != null)
            tileCollider.enabled = value;
    }

    public Tween CreateMatchEffect(float duration)
    {
        SetInteractable(false);

        return transform
            .DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack);
    }
}