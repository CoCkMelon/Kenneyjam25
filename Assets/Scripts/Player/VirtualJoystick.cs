using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float joystickRange = 100f;
    public float deadZone = 0.1f;
    public bool snapToCenter = true;
    
    [Header("Visual Settings")]
    public bool showAlways = false;
    public float fadeSpeed = 3f;
    
    private Vector2 inputVector;
    private bool isDragging = false;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    
    public Vector2 GetInputVector()
    {
        return inputVector.magnitude < deadZone ? Vector2.zero : inputVector;
    }
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        if (joystickBackground == null)
            joystickBackground = transform.GetChild(0).GetComponent<RectTransform>();
        if (joystickHandle == null)
            joystickHandle = joystickBackground.GetChild(0).GetComponent<RectTransform>();
        
        SetJoystickVisibility(showAlways);
    }
    
    void Update()
    {
        if (!showAlways)
        {
            float targetAlpha = isDragging ? 1f : 0f;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        
        if (!snapToCenter)
        {
            // Move joystick to touch position
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, 
                eventData.position, 
                parentCanvas.worldCamera, 
                out localPoint);
            
            joystickBackground.anchoredPosition = localPoint;
        }
        
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, 
            eventData.position, 
            parentCanvas.worldCamera, 
            out localPoint);
        
        // Calculate input vector
        inputVector = localPoint / joystickRange;
        
        // Clamp to circle
        if (inputVector.magnitude > 1f)
        {
            inputVector = inputVector.normalized;
            localPoint = inputVector * joystickRange;
        }
        
        // Apply dead zone
        if (inputVector.magnitude < deadZone)
        {
            inputVector = Vector2.zero;
        }
        
        // Update handle position
        joystickHandle.anchoredPosition = localPoint;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        inputVector = Vector2.zero;
        
        // Reset handle position
        joystickHandle.anchoredPosition = Vector2.zero;
        
        // If not snapping to center, reset background position
        if (!snapToCenter)
        {
            joystickBackground.anchoredPosition = Vector2.zero;
        }
    }
    
    public void SetJoystickVisibility(bool visible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
    
    public void SetJoystickRange(float range)
    {
        joystickRange = range;
    }
    
    public void SetDeadZone(float deadZone)
    {
        this.deadZone = Mathf.Clamp01(deadZone);
    }
    
    // Static method to create a virtual joystick from UI elements
    public static VirtualJoystick CreateFromUI(RectTransform background, RectTransform handle)
    {
        VirtualJoystick joystick = background.gameObject.AddComponent<VirtualJoystick>();
        joystick.joystickBackground = background;
        joystick.joystickHandle = handle;
        return joystick;
    }
}
