using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Handles power collection and management for the sleight
/// </summary>
public class SleightPowerSystem : MonoBehaviour
{
    [Header("Power Settings")]
    [SerializeField] private float currentPower = 0f;
    [SerializeField] private float maxPower = 1000f;
    [SerializeField] private float powerDecayRate = 0.5f; // Power lost per second when not collecting
    [SerializeField] private bool enablePowerDecay = false;
    
    [Header("Power Collection")]
    [SerializeField] private float collectionRadius = 2f;
    [SerializeField] private LayerMask powerOrbLayer = -1;
    [SerializeField] private float magneticRange = 5f;
    [SerializeField] private float magneticStrength = 10f;
    
    [Header("Power Multipliers")]
    [SerializeField] private float baseMultiplier = 1f;
    [SerializeField] private float comboMultiplier = 1f;
    [SerializeField] private float maxComboMultiplier = 5f;
    [SerializeField] private float comboDecayTime = 2f;
    
    [Header("Events")]
    public UnityEvent<float> OnPowerCollected;
    public UnityEvent<float> OnPowerLevelChanged;
    public UnityEvent<int> OnComboChanged;
    public UnityEvent OnMaxPowerReached;
    
    // Private fields
    private List<PowerOrb> nearbyOrbs = new List<PowerOrb>();
    private float lastCollectionTime = 0f;
    private int currentCombo = 0;
    private float comboTimer = 0f;
    
    // Properties
    public float CurrentPower => currentPower;
    public float MaxPower => maxPower;
    public float PowerPercentage => currentPower / maxPower;
    public int CurrentCombo => currentCombo;
    public float ComboMultiplier => comboMultiplier;
    
    void Update()
    {
        UpdatePowerCollection();
        UpdateComboSystem();
        UpdatePowerDecay();
    }
    
    private void UpdatePowerCollection()
    {
        // Find nearby power orbs
        Collider[] orbColliders = Physics.OverlapSphere(transform.position, magneticRange, powerOrbLayer);
        
        nearbyOrbs.Clear();
        foreach (Collider col in orbColliders)
        {
            PowerOrb orb = col.GetComponent<PowerOrb>();
            if (orb != null && orb.IsCollectable)
            {
                nearbyOrbs.Add(orb);
                
                // Apply magnetic effect
                float distance = Vector3.Distance(transform.position, orb.transform.position);
                if (distance <= magneticRange)
                {
                    Vector3 direction = (transform.position - orb.transform.position).normalized;
                    float magneticForce = magneticStrength * (1f - distance / magneticRange);
                    orb.ApplyMagneticForce(direction * magneticForce);
                }
                
                // Check for collection
                if (distance <= collectionRadius)
                {
                    CollectPowerOrb(orb);
                }
            }
        }
    }
    
    private void CollectPowerOrb(PowerOrb orb)
    {
        if (orb == null || !orb.IsCollectable) return;
        
        float powerAmount = orb.PowerValue;
        
        // Apply combo multiplier
        powerAmount *= comboMultiplier;
        
        // Add power
        AddPower(powerAmount);
        
        // Update combo
        UpdateCombo();
        
        // Mark orb as collected
        orb.Collect();
        
        // Notify collection
        OnPowerCollected?.Invoke(powerAmount);
        
        lastCollectionTime = Time.time;
    }
    
    private void UpdateComboSystem()
    {
        // Check if combo should continue
        if (Time.time - lastCollectionTime < comboDecayTime)
        {
            comboTimer = comboDecayTime - (Time.time - lastCollectionTime);
        }
        else
        {
            // Reset combo
            if (currentCombo > 0)
            {
                currentCombo = 0;
                comboMultiplier = baseMultiplier;
                OnComboChanged?.Invoke(currentCombo);
            }
            comboTimer = 0f;
        }
    }
    
    private void UpdateCombo()
    {
        currentCombo++;
        
        // Calculate combo multiplier
        float comboBonus = Mathf.Min(currentCombo * 0.1f, maxComboMultiplier - baseMultiplier);
        comboMultiplier = baseMultiplier + comboBonus;
        
        // Notify combo change
        OnComboChanged?.Invoke(currentCombo);
    }
    
    private void UpdatePowerDecay()
    {
        if (enablePowerDecay && currentPower > 0f)
        {
            // Only decay if not collecting recently
            if (Time.time - lastCollectionTime > 1f)
            {
                float decay = powerDecayRate * Time.deltaTime;
                currentPower = Mathf.Max(0f, currentPower - decay);
                OnPowerLevelChanged?.Invoke(currentPower);
            }
        }
    }
    
    public void AddPower(float amount)
    {
        if (amount <= 0f) return;
        
        float previousPower = currentPower;
        currentPower = Mathf.Min(maxPower, currentPower + amount);
        
        // Check if max power reached
        if (previousPower < maxPower && currentPower >= maxPower)
        {
            OnMaxPowerReached?.Invoke();
        }
        
        OnPowerLevelChanged?.Invoke(currentPower);
    }
    
    public void ConsumePower(float amount)
    {
        if (amount <= 0f) return;
        
        currentPower = Mathf.Max(0f, currentPower - amount);
        OnPowerLevelChanged?.Invoke(currentPower);
    }
    
    public void ResetPower()
    {
        currentPower = 0f;
        currentCombo = 0;
        comboMultiplier = baseMultiplier;
        lastCollectionTime = 0f;
        comboTimer = 0f;
        
        OnPowerLevelChanged?.Invoke(currentPower);
        OnComboChanged?.Invoke(currentCombo);
    }
    
    public void SetMaxPower(float newMaxPower)
    {
        maxPower = newMaxPower;
        currentPower = Mathf.Min(currentPower, maxPower);
        OnPowerLevelChanged?.Invoke(currentPower);
    }
    
    public bool HasPower(float amount)
    {
        return currentPower >= amount;
    }
    
    public float GetComboTimeRemaining()
    {
        return comboTimer;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw collection radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRadius);
        
        // Draw magnetic range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magneticRange);
        
        // Draw lines to nearby orbs
        Gizmos.color = Color.yellow;
        foreach (PowerOrb orb in nearbyOrbs)
        {
            if (orb != null)
            {
                Gizmos.DrawLine(transform.position, orb.transform.position);
            }
        }
    }
}
