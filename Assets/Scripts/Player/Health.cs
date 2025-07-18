using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool regenerateHealth = true;
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private float regenerationDelay = 5f;
    
    private float lastDamageTime;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;
    public float HealthPercentage => currentHealth / maxHealth;
    
    // Events
    public System.Action<float> OnHealthChanged;
    public System.Action OnDeath;
    public System.Action<float> OnDamageTaken;
    public System.Action<float> OnHealthRestored;
    
    void Start()
    {
        currentHealth = maxHealth;
        lastDamageTime = -regenerationDelay;
    }
    
    void Update()
    {
        if (regenerateHealth && currentHealth < maxHealth)
        {
            if (Time.time - lastDamageTime >= regenerationDelay)
            {
                RestoreHealth(regenerationRate * Time.deltaTime);
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (!IsAlive || damage <= 0) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        lastDamageTime = Time.time;
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void RestoreHealth(float amount)
    {
        if (amount <= 0) return;
        
        float oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        float actualRestored = currentHealth - oldHealth;
        if (actualRestored > 0)
        {
            OnHealthChanged?.Invoke(currentHealth);
            OnHealthRestored?.Invoke(actualRestored);
        }
    }
    
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void FullHeal()
    {
        RestoreHealth(maxHealth);
    }
    
    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} has died!");
        
        // You can add death effects here
        // For example: disable player controls, play death animation, etc.
    }
    
    // Method to respawn/revive the player
    public void Revive(float healthAmount = -1)
    {
        if (healthAmount <= 0)
            healthAmount = maxHealth;
            
        currentHealth = Mathf.Min(healthAmount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"{gameObject.name} has been revived!");
    }
    
    // Method to set current health (for save/load system)
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    // Event for respawn (called by GameLoader)
    public System.Action OnRespawn;
    
    // Trigger respawn event
    public void TriggerRespawn()
    {
        OnRespawn?.Invoke();
    }
}
