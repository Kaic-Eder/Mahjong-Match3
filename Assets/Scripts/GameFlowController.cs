using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private TrayAnimator trayAnimator;

    [Header("Regras")]
    [SerializeField] private int matchSize = 3;

    [Header("Estado atual")]
    [SerializeField] private GameState currentState = GameState.WaitingForInput;

    private readonly MatchFinder matchFinder = new MatchFinder();

    public GameState CurrentState => currentState;

    public bool CanReceiveInput =>
        currentState != GameState.GameOver &&
        currentState != GameState.Victory;

    public bool TrySelectTile(TileController tile)
    {
        if (!CanReceiveInput)
            return false;

        if (tile == null)
            return false;

        if (!slotManager.TryAddTile(tile))
            return false;

        // Impede que o mesmo tile seja selecionado novamente.
        tile.SetInteractable(false);

        // A lista lógica já foi atualizada.
        // Agora verifica imediatamente se surgiu um trio.
        if (matchFinder.TryFindMatch(
                slotManager.CurrentTiles,
                tile.TileTypeId,
                matchSize,
                out List<TileController> match))
        {
            StartMatchRemoval(match);
        }

        // O layout sempre é recalculado depois da seleção.
        // Se havia um layout anterior, TrayAnimator o cancela.
        trayAnimator.PlayReflow(slotManager.CurrentTiles);

        return true;
    }

    private void StartMatchRemoval(List<TileController> match)
    {
        if (match == null || match.Count == 0)
            return;

        Debug.Log("[Flow] Match encontrado; iniciando remoção visual.");
        ChangeState(GameState.ResolvingMatch);

        // Remove da lista AGORA, antes do reflow.
        // Os tiles continuam vivos para a animação de saída.
        slotManager.RemoveTiles(match);

        // Esta é a sequência independente do layout.
        // Um novo clique pode cancelar o layout, mas não esta remoção.
        Sequence removalSequence = trayAnimator.PlayMatchRemoval(match);

        removalSequence.OnComplete(() => FinishMatch(match));
    }

    private void FinishMatch(List<TileController> match)
    {
        Debug.Log("[Flow] Animação do match concluída; destruindo tiles.");

        // Este é o lugar correto para Destroy.
        // O callback só chegou aqui depois do fim da animação.
        foreach (TileController tile in match)
        {
            if (tile != null)
                Destroy(tile.gameObject);
        }

        // Não removemos da lista aqui: isso já aconteceu em StartMatchRemoval.
        // Não iniciamos outro reflow aqui: ele já foi iniciado imediatamente
        // depois de TrySelectTile detectar o match.
    }

    private void ChangeState(GameState nextState)
    {
        currentState = nextState;
        Debug.Log($"[Flow] Estado: {currentState}");
    }
}