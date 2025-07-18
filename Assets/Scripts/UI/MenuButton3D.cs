using System.Collections;
using UnityEngine;

public class MenuButton3D : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isAnimating = false;
    private MenuAction buttonAction;
    private MainMenu3D mainMenu;
    
    private Collider buttonCollider;
    
    private void Awake()
    {
        originalScale = transform.localScale;
        buttonCollider = GetComponent<Collider>();
        
        if (buttonCollider == null)
        {
            Debug.LogError($"No collider found on {gameObject.name}! Please add a collider component.");
        }
    }
    
    public void Initialize(MenuAction action, MainMenu3D menu)
    {
        buttonAction = action;
        mainMenu = menu;
    }
    
    private void OnMouseEnter()
    {
        if (!isAnimating)
        {
            isHovered = true;
            AnimateScale(originalScale * mainMenu.hoverScaleMultiplier, mainMenu.hoverTweenDuration);
        }
    }
    
    private void OnMouseExit()
    {
        if (!isAnimating && isHovered)
        {
            isHovered = false;
            AnimateScale(originalScale, mainMenu.hoverTweenDuration);
        }
    }
    
    private void OnMouseDown()
    {
        if (!isAnimating)
        {
            StartCoroutine(HandleClick());
        }
    }
    
    private IEnumerator HandleClick()
    {
        isAnimating = true;
        
        // Click animation - scale down
        yield return StartCoroutine(ScaleTo(originalScale * mainMenu.clickScaleMultiplier, mainMenu.clickTweenDuration));
        
        // Scale back up
        Vector3 targetScale = isHovered ? originalScale * mainMenu.hoverScaleMultiplier : originalScale;
        yield return StartCoroutine(ScaleTo(targetScale, mainMenu.clickTweenDuration));
        
        // Wait for action delay
        yield return new WaitForSeconds(mainMenu.actionDelay);
        
        // Execute the menu action
        if (mainMenu != null)
        {
            mainMenu.ExecuteMenuAction(buttonAction);
        }
        
        isAnimating = false;
    }
    
    private void AnimateScale(Vector3 targetScale, float duration)
    {
        StartCoroutine(ScaleTo(targetScale, duration));
    }
    
    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // Use smooth easing
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        transform.localScale = targetScale;
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
