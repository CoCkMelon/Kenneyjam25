using System.Collections;
using UnityEngine;

public class PuzzleButton3D : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float hoverYOffset = 0.2f;
    [SerializeField] private float clickYOffset = -0.1f;
    [SerializeField] private float hoverTweenDuration = 0.15f;
    [SerializeField] private float clickTweenDuration = 0.1f;
    
    private Vector3 originalPosition;
    private bool isHovered = false;
    private bool isAnimating = false;
    
    private Collider buttonCollider;
    
    private void Start()
    {
        originalPosition = transform.localPosition;
        buttonCollider = GetComponent<Collider>();
        
        if (buttonCollider == null)
        {
            Debug.LogError($"No collider found on {gameObject.name}! Please add a collider component.");
        }
    }
    
    private void OnMouseEnter()
    {
        if (!isAnimating)
        {
            isHovered = true;
            AnimatePosition(originalPosition + Vector3.up * hoverYOffset, hoverTweenDuration);
        }
    }
    
    private void OnMouseExit()
    {
        if (!isAnimating && isHovered)
        {
            isHovered = false;
            AnimatePosition(originalPosition, hoverTweenDuration);
        }
    }
    
    public void OnMouseClick()
    {
        if (!isAnimating)
        {
            StartCoroutine(HandleClick());
        }
    }
    
    private IEnumerator HandleClick()
    {
        isAnimating = true;
        
        // Click animation - move down
        yield return StartCoroutine(MoveTo(originalPosition + Vector3.up * clickYOffset, clickTweenDuration));
        
        // Move back to hover position or original position
        Vector3 targetPosition = isHovered ? originalPosition + Vector3.up * hoverYOffset : originalPosition;
        yield return StartCoroutine(MoveTo(targetPosition, clickTweenDuration));
        
        isAnimating = false;
    }
    
    private void AnimatePosition(Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveTo(targetPosition, duration));
    }
    
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
    
    // Public method to reset button position (useful when puzzle resets)
    public void ResetPosition()
    {
        StopAllCoroutines();
        transform.localPosition = originalPosition;
        isHovered = false;
        isAnimating = false;
    }
    
    // Public method to set new original position (useful for dynamic positioning)
    public void SetOriginalPosition(Vector3 newPosition)
    {
        originalPosition = newPosition;
        transform.localPosition = originalPosition;
    }
    
    private void OnValidate()
    {
        // Ensure we have a collider in the editor
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"{gameObject.name} needs a Collider component for mouse interaction!");
        }
    }
}
