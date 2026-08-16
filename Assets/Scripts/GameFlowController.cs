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

        if (!slotManager.TryAddTile(tile))
            return false;

        tile.SetInteractable(false);

        if (matchFinder.TryFindMatch(
                slotManager.CurrentTiles,
                tile.TileTypeId,
                matchSize,
                out List<TileController> match))
        {
            // A implementação desta separação será a próxima etapa.
            StartMatchRemoval(match);
        }

        trayAnimator.PlayReflow(slotManager.CurrentTiles);
        return true;
    }

    private void FinishTileEntry(TileController selectedTile)
    {
        Debug.Log("[Flow] Entrada na barra concluída.");

        if (!matchFinder.TryFindMatch(
                slotManager.CurrentTiles,
                selectedTile.TileTypeId,
                matchSize,
                out List<TileController> match))
        {
            ChangeState(GameState.WaitingForInput);
            return;
        }

        Debug.Log("[Flow] Match encontrado; iniciando animação de remoção.");
        ChangeState(GameState.ResolvingMatch);

        Sequence matchSequence = trayAnimator.PlayMatchRemoval(match);
        matchSequence.OnComplete(() => FinishMatch(match));
    }

    private void FinishMatch(List<TileController> match)
    {
        Debug.Log("[Flow] Animação do match concluída.");

        // Primeiro remove da estrutura lógica.
        slotManager.RemoveTiles(match);

        // Depois remove os objetos da cena.
        foreach (TileController tile in match)
        {
            if (tile != null)
                Destroy(tile.gameObject);
        }

        ChangeState(GameState.ReflowingTray);

        // Agora somente os sobreviventes são reposicionados.
        Sequence reflowSequence = trayAnimator.PlayReflow(slotManager.CurrentTiles);
        reflowSequence.OnComplete(FinishReflow);
    }

    private void FinishReflow()
    {
        Debug.Log("[Flow] Reflow concluído; input liberado.");
        ChangeState(GameState.WaitingForInput);
    }

    private void ChangeState(GameState nextState)
    {
        currentState = nextState;
        Debug.Log($"[Flow] Estado: {currentState}");
    }
}
