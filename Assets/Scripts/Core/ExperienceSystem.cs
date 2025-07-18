using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class LevelUpEvent : UnityEvent<int> { }

[System.Serializable]
public class ExperienceGainEvent : UnityEvent<int> { }

public class ExperienceSystem : MonoBehaviour
{
    [Header("Experience Settings")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int maxLevel = 50;
    public float experienceMultiplier = 1.0f;
    
    [Header("Level Progression")]
    public int baseExperienceRequired = 100;
    public float experienceScaling = 1.2f;
    
    [Header("Stat Bonuses Per Level")]
    public int healthBonusPerLevel = 10;
    public int manaBonusPerLevel = 5;
    public int attackBonusPerLevel = 2;
    public int defensePerLevel = 1;
    
    [Header("Events")]
    public LevelUpEvent OnLevelUp;
    public ExperienceGainEvent OnExperienceGain;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private int experienceToNextLevel;
    private Health playerHealth;
    private PlayerStats playerStats;
    
    // Cached values for performance
    private Dictionary<int, int> levelExperienceCache = new Dictionary<int, int>();
    
    void Start()
    {
        playerHealth = GetComponent<Health>();
        playerStats = GetComponent<PlayerStats>();
        
        // Initialize experience to next level
        CalculateExperienceToNextLevel();
        
        // Apply current level bonuses
        ApplyLevelBonuses();
        
        if (showDebugInfo)
        {
            Debug.Log($"Experience System initialized. Level: {currentLevel}, XP: {currentExperience}/{experienceToNextLevel}");
        }
    }
    
    public void GainExperience(int amount)
    {
        if (currentLevel >= maxLevel)
        {
            if (showDebugInfo)
                Debug.Log("Already at max level!");
            return;
        }
        
        int adjustedAmount = Mathf.RoundToInt(amount * experienceMultiplier);
        currentExperience += adjustedAmount;
        
        OnExperienceGain?.Invoke(adjustedAmount);
        
        if (showDebugInfo)
        {
            Debug.Log($"Gained {adjustedAmount} XP! Total: {currentExperience}/{experienceToNextLevel}");
        }
        
        // Check for level up
        CheckLevelUp();
    }
    
    void CheckLevelUp()
    {
        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }
    }
    
    void LevelUp()
    {
        currentExperience -= experienceToNextLevel;
        currentLevel++;
        
        // Apply level bonuses
        ApplyLevelBonuses();
        
        // Recalculate experience needed for next level
        CalculateExperienceToNextLevel();
        
        OnLevelUp?.Invoke(currentLevel);
        
        if (showDebugInfo)
        {
            Debug.Log($"LEVEL UP! New Level: {currentLevel}. Next level requires {experienceToNextLevel} XP");
        }
        
        // Show level up effect
        ShowLevelUpEffect();
    }
    
    void ApplyLevelBonuses()
    {
        if (playerHealth != null)
        {
            float newMaxHealth = 100f + (healthBonusPerLevel * (currentLevel - 1));
            playerHealth.SetMaxHealth(newMaxHealth);
            
            // Optional: heal player on level up
            playerHealth.FullHeal();
        }
        
        if (playerStats != null)
        {
            playerStats.UpdateStatsFromLevel(currentLevel);
        }
    }
    
    void CalculateExperienceToNextLevel()
    {
        if (currentLevel >= maxLevel)
        {
            experienceToNextLevel = 0;
            return;
        }
        
        // Use cached value if available
        if (levelExperienceCache.ContainsKey(currentLevel))
        {
            experienceToNextLevel = levelExperienceCache[currentLevel];
            return;
        }
        
        // Calculate and cache
        experienceToNextLevel = Mathf.RoundToInt(baseExperienceRequired * Mathf.Pow(experienceScaling, currentLevel - 1));
        levelExperienceCache[currentLevel] = experienceToNextLevel;
    }
    
    void ShowLevelUpEffect()
    {
        // TODO: Add particle effect, sound, UI animation
        // For now, just a simple visual cue
        
        // Try to find and trigger level up particle effect
        ParticleSystem levelUpEffect = GetComponentInChildren<ParticleSystem>();
        if (levelUpEffect != null)
        {
            levelUpEffect.Play();
        }
        
        // Play level up sound
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    
    public int GetExperienceForLevel(int level)
    {
        if (level <= 1) return 0;
        if (level > maxLevel) return GetTotalExperienceForMaxLevel();
        
        int totalExperience = 0;
        for (int i = 1; i < level; i++)
        {
            totalExperience += Mathf.RoundToInt(baseExperienceRequired * Mathf.Pow(experienceScaling, i - 1));
        }
        return totalExperience;
    }
    
    public int GetTotalExperienceForMaxLevel()
    {
        return GetExperienceForLevel(maxLevel);
    }
    
    public float GetExperienceProgress()
    {
        if (currentLevel >= maxLevel) return 1.0f;
        return (float)currentExperience / experienceToNextLevel;
    }
    
    public int GetExperienceToNextLevel()
    {
        return experienceToNextLevel - currentExperience;
    }
    
    public int GetTotalExperience()
    {
        return GetExperienceForLevel(currentLevel) + currentExperience;
    }
    
    // Methods for common experience sources
    public void GainExperienceFromKill(EnemyType enemyType)
    {
        int baseXP = GetExperienceForEnemyType(enemyType);
        GainExperience(baseXP);
    }
    
    public void GainExperienceFromQuest(int questReward)
    {
        GainExperience(questReward);
    }
    
    public void GainExperienceFromDiscovery(int discoveryBonus = 50)
    {
        GainExperience(discoveryBonus);
    }
    
    int GetExperienceForEnemyType(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Weak: return 10;
            case EnemyType.Normal: return 25;
            case EnemyType.Strong: return 50;
            case EnemyType.Elite: return 100;
            case EnemyType.Boss: return 500;
            default: return 15;
        }
    }
    
    // Save/Load support
    public ExperienceData GetExperienceData()
    {
        return new ExperienceData
        {
            level = currentLevel,
            experience = currentExperience,
            totalExperience = GetTotalExperience()
        };
    }
    
    public void LoadExperienceData(ExperienceData data)
    {
        currentLevel = data.level;
        currentExperience = data.experience;
        
        CalculateExperienceToNextLevel();
        ApplyLevelBonuses();
        
        if (showDebugInfo)
        {
            Debug.Log($"Experience data loaded. Level: {currentLevel}, XP: {currentExperience}");
        }
    }
    
    // Debug methods
    [ContextMenu("Add Test Experience")]
    void AddTestExperience()
    {
        GainExperience(50);
    }
    
    [ContextMenu("Level Up")]
    void TestLevelUp()
    {
        GainExperience(experienceToNextLevel);
    }
}

[System.Serializable]
public class ExperienceData
{
    public int level;
    public int experience;
    public int totalExperience;
}

public enum EnemyType
{
    Weak,
    Normal,
    Strong,
    Elite,
    Boss
}
