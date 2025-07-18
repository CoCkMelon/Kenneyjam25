using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    Health,
    Attack,
    Defense,
    Speed,
    Stamina,
    Mana,
    Luck,
    Experience
}

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseAttack = 10f;
    [SerializeField] private float baseDefense = 5f;
    [SerializeField] private float baseSpeed = 8f;
    [SerializeField] private float baseStamina = 50f;
    [SerializeField] private float baseMana = 30f;
    [SerializeField] private float baseLuck = 1f;
    
    [Header("Level System")]
    [SerializeField] private int level = 1;
    [SerializeField] private float experience = 0f;
    [SerializeField] private float experienceToNextLevel = 100f;
    [SerializeField] private float levelMultiplier = 1.5f;
    
    // Stat modifiers (from equipment, buffs, etc.)
    private Dictionary<StatType, float> statModifiers = new Dictionary<StatType, float>();
    
    // Events
    public System.Action<StatType, float> OnStatChanged;
    public System.Action<int> OnLevelUp;
    public System.Action<float> OnExperienceGained;
    
    void Start()
    {
        InitializeStats();
    }
    
    void InitializeStats()
    {
        // Initialize modifiers
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statModifiers[stat] = 0f;
        }
    }
    
    public float GetStat(StatType statType)
    {
        float baseStat = GetBaseStat(statType);
        float levelBonus = GetLevelBonus(statType);
        float modifier = statModifiers.ContainsKey(statType) ? statModifiers[statType] : 0f;
        
        return baseStat + levelBonus + modifier;
    }
    
    private float GetBaseStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Health: return baseHealth;
            case StatType.Attack: return baseAttack;
            case StatType.Defense: return baseDefense;
            case StatType.Speed: return baseSpeed;
            case StatType.Stamina: return baseStamina;
            case StatType.Mana: return baseMana;
            case StatType.Luck: return baseLuck;
            case StatType.Experience: return experience;
            default: return 0f;
        }
    }
    
    private float GetLevelBonus(StatType statType)
    {
        // Simple level scaling - you can make this more complex
        float levelBonus = (level - 1) * 2f;
        
        switch (statType)
        {
            case StatType.Health: return levelBonus * 10f;
            case StatType.Attack: return levelBonus * 1f;
            case StatType.Defense: return levelBonus * 0.5f;
            case StatType.Speed: return levelBonus * 0.3f;
            case StatType.Stamina: return levelBonus * 5f;
            case StatType.Mana: return levelBonus * 3f;
            case StatType.Luck: return levelBonus * 0.1f;
            case StatType.Experience: return 0f;
            default: return 0f;
        }
    }
    
    public void AddStatModifier(StatType statType, float value)
    {
        if (statModifiers.ContainsKey(statType))
        {
            statModifiers[statType] += value;
        }
        else
        {
            statModifiers[statType] = value;
        }
        
        OnStatChanged?.Invoke(statType, GetStat(statType));
    }
    
    public void RemoveStatModifier(StatType statType, float value)
    {
        if (statModifiers.ContainsKey(statType))
        {
            statModifiers[statType] -= value;
            OnStatChanged?.Invoke(statType, GetStat(statType));
        }
    }
    
    public void SetStatModifier(StatType statType, float value)
    {
        statModifiers[statType] = value;
        OnStatChanged?.Invoke(statType, GetStat(statType));
    }
    
    public void AddExperience(float amount)
    {
        experience += amount;
        OnExperienceGained?.Invoke(amount);
        
        // Check for level up
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        level++;
        experienceToNextLevel = Mathf.Round(experienceToNextLevel * levelMultiplier);
        
        OnLevelUp?.Invoke(level);
        
        // Update health component if it exists
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.SetMaxHealth(GetStat(StatType.Health));
        }
        
        Debug.Log($"Level up! Now level {level}");
    }
    
    // Method called by ExperienceSystem when level changes
    public void UpdateStatsFromLevel(int newLevel)
    {
        level = newLevel;
        
        // Update health component if it exists
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.SetMaxHealth(GetStat(StatType.Health));
        }
        
        // Notify all stat changes
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            OnStatChanged?.Invoke(stat, GetStat(stat));
        }
    }
    
    // Properties
    public int Level => level;
    public float Experience => experience;
    public float ExperienceToNextLevel => experienceToNextLevel;
    public float ExperiencePercentage => experience / experienceToNextLevel;
    
    // Method to set base stats (for character creation, etc.)
    public void SetBaseStat(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.Health: baseHealth = value; break;
            case StatType.Attack: baseAttack = value; break;
            case StatType.Defense: baseDefense = value; break;
            case StatType.Speed: baseSpeed = value; break;
            case StatType.Stamina: baseStamina = value; break;
            case StatType.Mana: baseMana = value; break;
            case StatType.Luck: baseLuck = value; break;
        }
        
        OnStatChanged?.Invoke(statType, GetStat(statType));
    }
    
    // Save/Load support
    public PlayerStatsData GetStatsData()
    {
        return new PlayerStatsData
        {
            level = level,
            baseHealth = baseHealth,
            baseMana = baseMana,
            baseAttack = baseAttack,
            baseDefense = baseDefense,
            baseSpeed = baseSpeed,
            baseLuck = baseLuck,
            baseStamina = baseStamina
        };
    }
    
    public void LoadStatsData(PlayerStatsData data)
    {
        level = data.level;
        baseHealth = data.baseHealth;
        baseMana = data.baseMana;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseStamina = data.baseStamina;
        
        // Update health component if it exists
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.SetMaxHealth(GetStat(StatType.Health));
        }
        
        // Notify all stat changes
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            OnStatChanged?.Invoke(stat, GetStat(stat));
        }
    }
}
