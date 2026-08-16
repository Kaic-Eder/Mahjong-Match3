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

    private Sequence layoutSequence;
    private Sequence removalSequence;

    public Sequence PlayReflow(IReadOnlyList<TileController> tiles)
    {
        KillLayoutAnimation();

        Sequence newLayout = DOTween.Sequence();
        bool addedTween = false;

        for (int i = 0; i < tiles.Count; i++)
        {
            TileController tile = tiles[i];

            if (tile == null)
                continue;

            if (!slotManager.TryGetSlotPosition(i, out Vector3 targetPosition))
                continue;

            float distance = Vector3.Distance(
                tile.transform.position,
                targetPosition);

            float duration = Mathf.Max(
                minimumMoveDuration,
                distance / moveSpeed);

            Tween moveTween = tile.transform
                .DOMove(targetPosition, duration)
                .SetEase(Ease.OutQuad);

            newLayout.Join(moveTween);
            addedTween = true;
        }

        if (!addedTween)
        {
            // Faz uma barra vazia ter uma conclusão previsível.
            newLayout.AppendCallback(() => { });
        }

        layoutSequence = newLayout;

        newLayout
            .OnComplete(() =>
            {
                if (layoutSequence == newLayout)
                    layoutSequence = null;
            })
            .OnKill(() =>
            {
                if (layoutSequence == newLayout)
                    layoutSequence = null;
            });

        return newLayout;
    }
    
    public void KillLayoutAnimation()
    {
        if (layoutSequence != null && layoutSequence.IsActive())
        {
            // false significa: não complete o destino antigo.
            // O tile fica exatamente onde estava quando o movimento foi interrompido.
            layoutSequence.Kill(false);
        }

        layoutSequence = null;
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

    private void OnDestroy()
    {
        KillLayoutAnimation();
    }
}
