using UnityEngine;

/// <summary>
/// Collectible power orb that provides power to the sleight
/// </summary>
public class PowerOrb : MonoBehaviour
{
    [Header("Power Orb Settings")]
    [SerializeField] private float powerValue = 10f;
    [SerializeField] private OrbType orbType = OrbType.Basic;
    [SerializeField] private float lifeTime = 30f;
    [SerializeField] private bool hasLifeTime = true;
    
    [Header("Movement")]
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject visualModel;
    
    [Header("Magnetic Properties")]
    [SerializeField] private float magneticResistance = 1f;
    [SerializeField] private float maxMagneticSpeed = 10f;
    
    // Private fields
    private Vector3 initialPosition;
    private bool isCollectable = true;
    private float creationTime;
    private Rigidbody rb;
    private Vector3 magneticForce = Vector3.zero;
    
    // Properties
    public float PowerValue => powerValue;
    public OrbType OrbType => orbType;
    public bool IsCollectable => isCollectable;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure rigidbody for magnetic attraction
        // rb.useGravity = false;
        // rb.linearDamping = 2f;
        // rb.angularDamping = 5f;
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        initialPosition = transform.position;
        creationTime = Time.time;
        
        // Set power value based on orb type
        SetPowerValueByType();
        
        // Setup visual effects
        SetupVisualEffects();
    }
    
    void Update()
    {
        if (!isCollectable) return;
        
        UpdateFloatingMotion();
        UpdateRotation();
        UpdateLifeTime();
        ApplyMagneticMovement();
    }
    
    private void SetPowerValueByType()
    {
        switch (orbType)
        {
            case OrbType.Basic:
                powerValue = 10f;
                break;
            case OrbType.Enhanced:
                powerValue = 25f;
                break;
            case OrbType.Rare:
                powerValue = 50f;
                break;
            case OrbType.Epic:
                powerValue = 100f;
                break;
            case OrbType.Legendary:
                powerValue = 250f;
                break;
        }
    }
    
    private void SetupVisualEffects()
    {
        // Change visual appearance based on orb type
        if (visualModel != null)
        {
            Renderer renderer = visualModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color orbColor = GetOrbColor();
                renderer.material.color = orbColor;
                
                // Add emission for higher tier orbs
                if (orbType >= OrbType.Rare)
                {
                    renderer.material.SetColor("_EmissionColor", orbColor * 0.5f);
                    renderer.material.EnableKeyword("_EMISSION");
                }
            }
        }
        
        // Setup particle effects
        if (collectEffect != null)
        {
            var main = collectEffect.main;
            main.startColor = GetOrbColor();
        }
    }
    
    private Color GetOrbColor()
    {
        switch (orbType)
        {
            case OrbType.Basic: return Color.white;
            case OrbType.Enhanced: return Color.blue;
            case OrbType.Rare: return Color.green;
            case OrbType.Epic: return Color.magenta;
            case OrbType.Legendary: return Color.yellow;
            default: return Color.white;
        }
    }
    
    private void UpdateFloatingMotion()
    {
        if (rb != null && magneticForce.magnitude < 0.1f)
        {
            // Only apply floating motion if not being magnetically attracted
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            Vector3 targetPosition = initialPosition + Vector3.up * yOffset;
            
            Vector3 floatForce = (targetPosition - transform.position) * 2f;
            rb.AddForce(floatForce, ForceMode.Force);
        }
    }
    
    private void UpdateRotation()
    {
        if (rb != null)
        {
            rb.angularVelocity = Vector3.up * rotationSpeed * Mathf.Deg2Rad;
        }
    }
    
    private void UpdateLifeTime()
    {
        if (hasLifeTime && Time.time - creationTime >= lifeTime)
        {
            DestroyOrb();
        }
    }
    
    private void ApplyMagneticMovement()
    {
        if (rb != null && magneticForce.magnitude > 0.1f)
        {
            Vector3 adjustedForce = magneticForce / magneticResistance;
            rb.AddForce(adjustedForce, ForceMode.Force);
            
            // Limit magnetic speed
            if (rb.linearVelocity.magnitude > maxMagneticSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxMagneticSpeed;
            }
        }
        
        // Reset magnetic force (will be set again by power system if needed)
        magneticForce = Vector3.zero;
    }
    
    public void ApplyMagneticForce(Vector3 force)
    {
        magneticForce = force;
    }
    
    public void Collect()
    {
        if (!isCollectable) return;
        
        isCollectable = false;
        
        // Play collection effects
        if (collectEffect != null)
        {
            collectEffect.Play();
        }
        
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // Hide visual model
        if (visualModel != null)
        {
            visualModel.SetActive(false);
        }
        
        // Disable collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Destroy after effects finish
        Destroy(gameObject, 2f);
    }
    
    public void SetPowerValue(float value)
    {
        powerValue = value;
    }
    
    public void SetOrbType(OrbType type)
    {
        orbType = type;
        SetPowerValueByType();
        SetupVisualEffects();
    }
    
    public void SetLifeTime(float time)
    {
        lifeTime = time;
        hasLifeTime = time > 0f;
    }
    
    private void DestroyOrb()
    {
        // Fade out effect could be added here
        Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Additional trigger-based collection (backup to distance-based collection)
        if (other.CompareTag("Player"))
        {
            SleightPowerSystem powerSystem = other.GetComponent<SleightPowerSystem>();
            if (powerSystem != null && isCollectable)
            {
                Collect();
            }
        }
    }
}

[System.Serializable]
public enum OrbType
{
    Basic,
    Enhanced,
    Rare,
    Epic,
    Legendary
}
