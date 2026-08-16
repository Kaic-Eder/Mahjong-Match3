using DG.Tweening;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [Header("Identidade")]
    [SerializeField] private int tileTypeId;

    [Header("Referências locais")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D tileCollider;

    public int TileTypeId => tileTypeId;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (tileCollider == null)
            tileCollider = GetComponent<Collider2D>();

        // Mantém a lógica que já existe no seu projeto.
        // Mais adiante, você poderá substituir isso por TileDefinition.
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            string spriteName = spriteRenderer.sprite.name;
            string numericPart = spriteName.Remove(0, 6);

            if (!int.TryParse(numericPart, out tileTypeId))
            {
                Debug.LogError(
                    $"Não foi possível descobrir o tipo do tile pelo sprite {spriteName}.",
                    this);
            }
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

        // Comece apenas com escala. Depois você pode incluir DOFade.
        return transform
            .DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack);
    }
}