using UnityEngine;

public class WindForce : MonoBehaviour
{
    [Header("Wind Settings")]
    [Tooltip("Base wind direction (normalized)")]
    public Vector3 baseWindDirection = Vector3.right;
    
    [Tooltip("Base wind strength")]
    public float baseWindStrength = 5f;
    
    [Header("Randomization")]
    [Tooltip("How much the wind direction can vary (0-1)")]
    [Range(0f, 1f)]
    public float directionVariation = 0.3f;
    
    [Tooltip("How much the wind strength can vary (0-1)")]
    [Range(0f, 1f)]
    public float strengthVariation = 0.5f;
    
    [Header("Wind Gusts")]
    [Tooltip("Enable wind gusts for more dynamic wind")]
    public bool enableGusts = true;
    
    [Tooltip("Minimum time between gusts")]
    public float gustMinInterval = 2f;
    
    [Tooltip("Maximum time between gusts")]
    public float gustMaxInterval = 8f;
    
    [Tooltip("Gust strength multiplier")]
    public float gustStrengthMultiplier = 2f;
    
    [Tooltip("How long a gust lasts")]
    public float gustDuration = 1f;
    
    [Header("Wind Zones")]
    [Tooltip("Use a trigger collider to define wind zone (optional)")]
    public bool useWindZone = false;
    
    [Tooltip("Apply wind force continuously or in intervals")]
    public bool continuousWind = true;
    
    [Tooltip("Force application interval (if not continuous)")]
    public float forceInterval = 0.1f;
    
    [Header("Debug")]
    [Tooltip("Show wind direction in scene view")]
    public bool showWindDebug = true;
    
    [Tooltip("Length of debug arrow")]
    public float debugArrowLength = 2f;
    
    private Rigidbody targetRigidbody;
    private Vector3 currentWindDirection;
    private float currentWindStrength;
    private bool isGustActive = false;
    private float gustTimer = 0f;
    private float nextGustTime;
    private float forceTimer = 0f;
    
    // For wind zone mode
    private bool isInWindZone = false;
    
    private void Start()
    {
        // Get rigidbody component
        targetRigidbody = GetComponent<Rigidbody>();
        
        if (targetRigidbody == null)
        {
            Debug.LogError($"WindForce script on {gameObject.name} requires a Rigidbody component!");
            enabled = false;
            return;
        }
        
        // Initialize wind parameters
        UpdateWindParameters();
        
        // Set up first gust timer
        if (enableGusts)
        {
            nextGustTime = Random.Range(gustMinInterval, gustMaxInterval);
        }
        
        // If using wind zone, check for trigger collider
        if (useWindZone)
        {
            Collider col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                Debug.LogWarning($"WindForce on {gameObject.name} is set to use wind zone but collider is not a trigger!");
            }
        }
    }
    
    private void Update()
    {
        // Handle gust system
        if (enableGusts)
        {
            HandleGusts();
        }
        
        // Update wind parameters periodically for natural variation
        UpdateWindParameters();
    }
    
    private void FixedUpdate()
    {
        // Apply wind force
        if (ShouldApplyWind())
        {
            ApplyWindForce();
        }
    }
    
    private bool ShouldApplyWind()
    {
        // Check if we should apply wind based on various conditions
        if (targetRigidbody == null) return false;
        
        // If using wind zone, only apply when in zone
        if (useWindZone && !isInWindZone) return false;
        
        // Check force interval timing
        if (!continuousWind)
        {
            forceTimer += Time.fixedDeltaTime;
            if (forceTimer < forceInterval) return false;
            forceTimer = 0f;
        }
        
        return true;
    }
    
    private void ApplyWindForce()
    {
        Vector3 windForce = currentWindDirection * currentWindStrength;
        
        // Apply gust multiplier if active
        if (isGustActive)
        {
            windForce *= gustStrengthMultiplier;
        }
        
        // Apply the force
        targetRigidbody.AddForce(windForce, ForceMode.Force);
    }
    
    private void UpdateWindParameters()
    {
        // Update wind direction with variation
        Vector3 randomDirection = Random.insideUnitSphere * directionVariation;
        currentWindDirection = (baseWindDirection + randomDirection).normalized;
        
        // Update wind strength with variation
        float strengthVar = Random.Range(-strengthVariation, strengthVariation);
        currentWindStrength = baseWindStrength * (1f + strengthVar);
        
        // Ensure strength is never negative
        currentWindStrength = Mathf.Max(0f, currentWindStrength);
    }
    
    private void HandleGusts()
    {
        if (isGustActive)
        {
            // Count down gust duration
            gustTimer -= Time.deltaTime;
            if (gustTimer <= 0f)
            {
                isGustActive = false;
                nextGustTime = Random.Range(gustMinInterval, gustMaxInterval);
            }
        }
        else
        {
            // Count down to next gust
            nextGustTime -= Time.deltaTime;
            if (nextGustTime <= 0f)
            {
                isGustActive = true;
                gustTimer = gustDuration;
            }
        }
    }
    
    // Wind zone trigger events
    private void OnTriggerEnter(Collider other)
    {
        if (useWindZone && other.GetComponent<Rigidbody>() == targetRigidbody)
        {
            isInWindZone = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (useWindZone && other.GetComponent<Rigidbody>() == targetRigidbody)
        {
            isInWindZone = false;
        }
    }
    
    // Public methods for external control
    public void SetWindDirection(Vector3 direction)
    {
        baseWindDirection = direction.normalized;
    }
    
    public void SetWindStrength(float strength)
    {
        baseWindStrength = Mathf.Max(0f, strength);
    }
    
    public void TriggerGust()
    {
        if (enableGusts)
        {
            isGustActive = true;
            gustTimer = gustDuration;
        }
    }
    
    public void StopWind()
    {
        enabled = false;
    }
    
    public void StartWind()
    {
        enabled = true;
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        if (!showWindDebug) return;
        
        // Draw wind direction arrow
        Gizmos.color = isGustActive ? Color.red : Color.cyan;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + currentWindDirection * debugArrowLength;
        
        Gizmos.DrawLine(startPos, endPos);
        
        // Draw arrow head
        Vector3 arrowHead1 = endPos - currentWindDirection * 0.3f + Vector3.up * 0.1f;
        Vector3 arrowHead2 = endPos - currentWindDirection * 0.3f - Vector3.up * 0.1f;
        
        Gizmos.DrawLine(endPos, arrowHead1);
        Gizmos.DrawLine(endPos, arrowHead2);
        
        // Draw wind zone if enabled
        if (useWindZone)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.DrawCube(transform.position, col.bounds.size);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showWindDebug) return;
        
        // Draw base wind direction in orange when selected
        Gizmos.color = Color.yellow;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + baseWindDirection * debugArrowLength;
        
        Gizmos.DrawLine(startPos, endPos);
    }
}
