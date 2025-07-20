using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// 3D Math Button that can display text/numbers and respond to clicks
/// Inherits animation behavior from PuzzleButton3D but adds text display
/// </summary>
public class MathButton3D : MonoBehaviour, IClickable
{
    [Header("Button Content")]
    [SerializeField] private GameObject buttonSymbol;
    [SerializeField] private string buttonValue = "";
    
    [Header("Animation Settings")]
    [SerializeField] private float hoverYOffset = 0.2f;
    [SerializeField] private float clickYOffset = -0.1f;
    [SerializeField] private float hoverTweenDuration = 0.15f;
    [SerializeField] private float clickTweenDuration = 0.1f;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.cyan;
    [SerializeField] private Color clickColor = Color.yellow;
    
    // State
    private Vector3 originalPosition;
    private bool isHovered = false;
    private bool isAnimating = false;
    private Action onClickCallback;
    
    // Components
    private Collider buttonCollider;
    private Renderer buttonRenderer;
    private Material buttonMaterial;
    private Color originalMaterialColor;
    
    private void Start()
    {
        SetupComponents();
        originalPosition = transform.localPosition;
    }
    
    private void SetupComponents()
    {
        // Get symbol child object
        if (buttonSymbol == null)
        {
            buttonSymbol = transform.GetChild(0)?.gameObject;
        }
        
        // Get collider
        buttonCollider = GetComponent<Collider>();
        if (buttonCollider == null)
        {
            // Add a default box collider if none exists
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(1f, 1f, 0.2f);
        }
        
        // Get renderer and material for color changes
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer != null && buttonRenderer.material != null)
        {
            buttonMaterial = buttonRenderer.material;
            originalMaterialColor = buttonMaterial.color;
        }
    }
    
    /// <summary>
    /// Initialize the button with symbol prefab and click callback
    /// </summary>
    public void Initialize(string value, Action clickCallback)
    {
        buttonValue = value;
        onClickCallback = clickCallback;
    }
    
    /// <summary>
    /// Initialize the button with 3D symbol prefab and click callback
    /// </summary>
    public void Initialize(string value, GameObject symbolPrefab, Action clickCallback)
    {
        buttonValue = value;
        onClickCallback = clickCallback;
        
        // Clear existing symbol
        if (buttonSymbol != null)
        {
            DestroyImmediate(buttonSymbol);
        }
        
        // Create new symbol
        if (symbolPrefab != null)
        {
            buttonSymbol = Instantiate(symbolPrefab, transform);
            buttonSymbol.transform.localPosition = Vector3.zero;
            buttonSymbol.transform.localRotation = Quaternion.identity;
            buttonSymbol.transform.localScale = Vector3.one;
        }
    }
    
    /// <summary>
    /// Handle mouse hover enter (for traditional Unity mouse events)
    /// </summary>
    private void OnMouseEnter()
    {
        if (!isAnimating)
        {
            isHovered = true;
            AnimatePosition(originalPosition + Vector3.up * hoverYOffset, hoverTweenDuration);
            SetButtonColor(hoverColor);
        }
    }
    
    /// <summary>
    /// Handle mouse hover exit (for traditional Unity mouse events)
    /// </summary>
    private void OnMouseExit()
    {
        if (!isAnimating && isHovered)
        {
            isHovered = false;
            AnimatePosition(originalPosition, hoverTweenDuration);
            SetButtonColor(normalColor);
        }
    }
    
    /// <summary>
    /// Handle mouse down (for traditional Unity mouse events)
    /// </summary>
    private void OnMouseDown()
    {
        if (!isAnimating)
        {
            StartCoroutine(HandleClick());
        }
    }
    
    /// <summary>
    /// Handle click via IClickable interface (for InputModeController)
    /// </summary>
    public void OnClicked()
    {
        if (!isAnimating)
        {
            StartCoroutine(HandleClick());
        }
    }
    
    /// <summary>
    /// Handle the click animation and callback
    /// </summary>
    private IEnumerator HandleClick()
    {
        isAnimating = true;
        
        // Change color to indicate click
        SetButtonColor(clickColor);
        
        // Click animation - move down
        yield return StartCoroutine(MoveTo(originalPosition + Vector3.up * clickYOffset, clickTweenDuration));
        
        // Invoke the callback
        onClickCallback?.Invoke();
        
        // Move back to hover position or original position
        Vector3 targetPosition = isHovered ? originalPosition + Vector3.up * hoverYOffset : originalPosition;
        yield return StartCoroutine(MoveTo(targetPosition, clickTweenDuration));
        
        // Reset color
        SetButtonColor(isHovered ? hoverColor : normalColor);
        
        isAnimating = false;
    }
    
    /// <summary>
    /// Animate button position
    /// </summary>
    private void AnimatePosition(Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveTo(targetPosition, duration));
    }
    
    /// <summary>
    /// Smoothly move button to target position
    /// </summary>
    private IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // Use smooth easing
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.localPosition = targetPosition;
    }
    
    /// <summary>
    /// Set button material color
    /// </summary>
    private void SetButtonColor(Color color)
    {
        if (buttonMaterial != null)
        {
            buttonMaterial.color = color;
        }
    }
    
    /// <summary>
    /// Reset button to original state
    /// </summary>
    public void ResetButton()
    {
        StopAllCoroutines();
        transform.localPosition = originalPosition;
        isHovered = false;
        isAnimating = false;
        SetButtonColor(normalColor);
    }
    
    /// <summary>
    /// Set button enabled/disabled state
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        if (buttonCollider != null)
        {
            buttonCollider.enabled = enabled;
        }
        
        // Enable/disable symbol visibility
        if (buttonSymbol != null)
        {
            buttonSymbol.SetActive(enabled);
        }
        
        if (buttonMaterial != null)
        {
            Color targetColor = enabled ? normalColor : Color.gray;
            buttonMaterial.color = targetColor;
        }
    }
    
    /// <summary>
    /// Get the button's text value
    /// </summary>
    public string GetValue()
    {
        return buttonValue;
    }
    
    /// <summary>
    /// Set the button's text value
    /// </summary>
    public void SetValue(string value)
    {
        buttonValue = value;
        // Note: 3D symbol changing would require recreating the symbol
        // This is more complex and typically done during initialization
    }
    
    /// <summary>
    /// Validate component setup in editor
    /// </summary>
    private void OnValidate()
    {
        // Ensure we have a collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"{gameObject.name} needs a Collider component for interaction!");
        }
    }
}
