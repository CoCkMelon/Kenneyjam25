using UnityEngine;

/// <summary>
/// Controls input modes - FPV navigation vs UI interaction
/// Handles cursor lock state and mouse picking conflicts
/// </summary>
public class InputModeController : MonoBehaviour
{
    public static InputModeController Instance { get; private set; }
    
    [Header("Input Modes")]
    public bool startInFPVMode = true;
    public KeyCode toggleModeKey = KeyCode.Tab;
    
    [Header("FPV Settings")]
    public bool lockCursorInFPV = true;
    
    [Header("UI Interaction Settings")]
    public Camera playerCamera;
    public LayerMask interactableLayerMask = -1;
    public float maxInteractionDistance = 10f;
    
    // Current state
    public bool IsFPVMode { get; private set; }
    public bool IsUIMode => !IsFPVMode;
    
    // Components
    private FirstPersonController fpController;
    private PlayerInput playerInput;
    
    // Events
    public System.Action<bool> OnModeChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Get components
        fpController = GetComponent<FirstPersonController>();
        playerInput = GetComponent<PlayerInput>();
        
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Set initial mode
        SetFPVMode(startInFPVMode);
    }
    
    void Update()
    {
        HandleModeToggle();
        
        if (IsUIMode)
        {
            HandleUIInteraction();
        }
    }
    
    private void HandleModeToggle()
    {
        if (Input.GetKeyDown(toggleModeKey))
        {
            ToggleMode();
        }
    }
    
    public void ToggleMode()
    {
        SetFPVMode(!IsFPVMode);
    }
    
    public void SetFPVMode(bool fpvMode)
    {
        IsFPVMode = fpvMode;
        
        // Handle cursor state
        if (IsFPVMode && lockCursorInFPV)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Enable/disable FPV controls
        if (playerInput != null)
        {
            playerInput.enableMouseLook = IsFPVMode;
        }
        
        // Notify listeners
        OnModeChanged?.Invoke(IsFPVMode);
        
        Debug.Log($"Input mode changed to: {(IsFPVMode ? "FPV" : "UI")}");
    }
    
    private void HandleUIInteraction()
    {
        // Handle mouse clicks in UI mode with screen center raycasting when needed
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }
    
    private void HandleMouseClick()
    {
        Ray ray;
        
        // If we were previously in FPV mode and cursor was locked,
        // cast from screen center instead of mouse position
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        }
        else
        {
            ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        }
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance, interactableLayerMask))
        {
            // Try different interaction interfaces
            var clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClicked();
                return;
            }
            
            // Fallback to MonoBehaviour with SendMessage
            hit.collider.SendMessage("OnMouseClick", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    // Method to temporarily unlock cursor (useful for puzzles)
    public void TemporaryUnlockCursor(float duration = 0f)
    {
        StartCoroutine(TemporaryUnlockCoroutine(duration));
    }
    
    private System.Collections.IEnumerator TemporaryUnlockCoroutine(float duration)
    {
        bool wasFPVMode = IsFPVMode;
        SetFPVMode(false);
        
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            SetFPVMode(wasFPVMode);
        }
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

// Interface for clickable objects
public interface IClickable
{
    void OnClicked();
}
