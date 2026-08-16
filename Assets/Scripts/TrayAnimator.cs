using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrayAnimator : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private SlotManager slotManager;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float minimumMoveDuration = 0.08f;

    [Header("Match")]
    [SerializeField] private float matchDuration = 0.25f;

    private Sequence activeSequence;

    public Sequence PlayReflow(IReadOnlyList<TileController> tiles)
    {
        Sequence sequence = DOTween.Sequence();

        bool addedTween = false;

        for (int i = 0; i < tiles.Count; i++)
        {
            TileController tile = tiles[i];

            if (tile == null)
                continue;

            if (!slotManager.TryGetSlotPosition(i, out Vector3 targetPosition))
                continue;

            float distance = Vector3.Distance(tile.transform.position, targetPosition);
            float duration = Mathf.Max(minimumMoveDuration, distance / moveSpeed);

            Tween moveTween = tile.transform
                .DOMove(targetPosition, duration)
                .SetEase(Ease.OutQuad);

            sequence.Join(moveTween);
            addedTween = true;
        }

        // Evita deixar uma Sequence vazia quando a barra não tem tiles.
        if (!addedTween)
            sequence.AppendCallback(() => { });

        return sequence;
    }

    public Sequence PlayMatchRemoval(IReadOnlyList<TileController> matchedTiles)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (TileController tile in matchedTiles)
        {
            if (tile == null)
                continue;

            sequence.Join(tile.CreateMatchEffect(matchDuration));
        }

        return sequence;
    }

    public void CancelActiveAnimation()
    {
        if (activeSequence != null && activeSequence.IsActive())
            activeSequence.Kill();

        activeSequence = null;
    }

    private void OnDestroy()
    {
        CancelActiveAnimation();
    }
}