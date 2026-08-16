using UnityEngine;
using UnityEngine.InputSystem;

public class TileInteractionHandler : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private GameFlowController gameFlow;

    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.PrimaryContact.started += OnTouchStarted;
    }

    private void OnDisable()
    {
        inputActions.Player.PrimaryContact.started -= OnTouchStarted;
        inputActions.Player.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = inputActions.Player.PrimaryPosition.ReadValue<Vector2>();
        DetectTile(screenPosition);
    }

    private void DetectTile(Vector2 screenPosition)
    {
        if (gameFlow == null || !gameFlow.CanReceiveInput)
            return;

        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(
            worldPoint,
            Vector2.zero,
            Mathf.Infinity,
            tileLayer);

        if (!hit.collider)
            return;

        if (!hit.collider.TryGetComponent(out TileController tile))
            return;

        // O próprio fluxo decide se a seleção foi aceita.
        // Não desabilite o collider aqui.
        gameFlow.TrySelectTile(tile);
    }
}