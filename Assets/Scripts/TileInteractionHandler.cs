using UnityEngine;
using UnityEngine.InputSystem;

public class TileInteractionHandler : MonoBehaviour
{
    [Header("Referências")] 
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private SlotManager slotManager;

    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.PrimaryContact.started += OnTouchStarted;
        //inputActions.Player.PrimaryContact.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputActions.Player.PrimaryContact.started -= OnTouchStarted;
        //inputActions.Player.PrimaryContact.canceled -= OnTouchEnded;
        
        inputActions.Player.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext ctx)
    {
        Vector2 screenPosition = inputActions.Player.PrimaryPosition.ReadValue<Vector2>();
        
        DetectTile(screenPosition);
    }

    private void DetectTile(Vector2 screenPosition)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, tileLayer);

        if (hit.collider != null)
        {
            TileController tile = hit.transform.gameObject.GetComponent<TileController>();
            slotManager.TryAddTile(tile);
            hit.collider.gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
    
}
