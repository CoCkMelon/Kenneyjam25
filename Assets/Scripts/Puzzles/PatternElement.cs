using UnityEngine;

/// <summary>
/// Individual pattern element for the memory puzzle
/// </summary>
public class PatternElement : MonoBehaviour
{
    [Header("Element Settings")]
    [SerializeField] private int elementIndex;
    [SerializeField] private PatternElementType elementType = PatternElementType.Standard;
    
    [Header("Visual Components")]
    [SerializeField] private Renderer elementRenderer;
    [SerializeField] private Light elementLight;
    [SerializeField] private ParticleSystem activationEffect;
    [SerializeField] private float activationDuration = 0.5f;
    
    [Header("Materials")]
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material errorMaterial;
    [SerializeField] private Material completedMaterial;
    
    [Header("Animation")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
    [SerializeField] private float animationSpeed = 2f;
    
    // Private fields
    private MemoryPatternPuzzle parentPuzzle;
    private bool isActive = false;
    private bool isCompleted = false;
    private bool isInErrorState = false;
    private Vector3 originalScale;
    private Color originalColor;
    private float animationTimer = 0f;
    
    // Properties
    public int ElementIndex => elementIndex;
    public PatternElementType ElementType => elementType;
    public bool IsActive => isActive;
    public bool IsCompleted => isCompleted;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        if (elementRenderer != null)
            originalColor = elementRenderer.material.color;
        
        // Setup collider if not present
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        
        UpdateVisuals();
    }
    
    void Update()
    {
        if (isActive || isCompleted)
        {
            AnimateElement();
        }
    }
    
    public void Initialize(MemoryPatternPuzzle puzzle, int index)
    {
        parentPuzzle = puzzle;
        elementIndex = index;
        
        UpdateVisuals();
    }
    
    public void Activate()
    {
        isActive = true;
        animationTimer = 0f;
        
        // Play activation effect
        if (activationEffect != null)
        {
            activationEffect.Play();
        }
        
        UpdateVisuals();
    }
    
    public void Deactivate()
    {
        isActive = false;
        animationTimer = 0f;
        
        // Stop activation effect
        if (activationEffect != null)
        {
            activationEffect.Stop();
        }
        
        UpdateVisuals();
    }
    
    public void SetErrorState(bool error)
    {
        isInErrorState = error;
        UpdateVisuals();
    }
    
    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        if (completed)
        {
            isActive = false;
            isInErrorState = false;
        }
        UpdateVisuals();
    }
    
    public void Reset()
    {
        isActive = false;
        isCompleted = false;
        isInErrorState = false;
        animationTimer = 0f;
        
        if (activationEffect != null)
            activationEffect.Stop();
        
        transform.localScale = originalScale;
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (elementRenderer != null)
        {
            Material targetMaterial = inactiveMaterial;
            
            if (isCompleted)
                targetMaterial = completedMaterial;
            else if (isInErrorState)
                targetMaterial = errorMaterial;
            else if (isActive)
                targetMaterial = activeMaterial;
            
            elementRenderer.material = targetMaterial;
        }
        
        // Update light
        if (elementLight != null)
        {
            elementLight.enabled = isActive || isCompleted;
            
            if (isCompleted)
            {
                elementLight.color = Color.green;
                elementLight.intensity = 1f;
            }
            else if (isInErrorState)
            {
                elementLight.color = Color.red;
                elementLight.intensity = 1.5f;
            }
            else if (isActive)
            {
                elementLight.color = GetElementColor();
                elementLight.intensity = 1.2f;
            }
        }
    }
    
    private Color GetElementColor()
    {
        switch (elementType)
        {
            case PatternElementType.Standard: return Color.white;
            case PatternElementType.Fire: return Color.red;
            case PatternElementType.Water: return Color.blue;
            case PatternElementType.Earth: return Color.green;
            case PatternElementType.Air: return Color.cyan;
            case PatternElementType.Light: return Color.yellow;
            case PatternElementType.Dark: return Color.magenta;
            default: return Color.white;
        }
    }
    
    private void AnimateElement()
    {
        animationTimer += Time.deltaTime * animationSpeed;
        
        if (isActive)
        {
            // Pulse animation
            float pulse = scaleCurve.Evaluate(Mathf.PingPong(animationTimer, 1f));
            transform.localScale = originalScale * pulse;
        }
        else if (isCompleted)
        {
            // Gentle floating animation
            float float_y = Mathf.Sin(animationTimer) * 0.1f;
            transform.localScale = originalScale * (1f + float_y);
        }
    }
    
    void OnMouseEnter()
    {
        if (!isActive && !isCompleted)
        {
            // Slight highlight on hover
            transform.localScale = originalScale * 1.05f;
        }
    }
    
    void OnMouseExit()
    {
        if (!isActive && !isCompleted)
        {
            transform.localScale = originalScale;
        }
    }
    
    void OnMouseDown()
    {
        if (parentPuzzle != null && parentPuzzle.IsWaitingForInput)
        {
            // The puzzle will handle the click through raycasting
            // This is just a backup method
        }
    }
}

public enum PatternElementType
{
    Standard,
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}
