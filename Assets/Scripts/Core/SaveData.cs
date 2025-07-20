using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Header("Player Data")]
    public Vector3 playerPosition;
    public float playerHealth;
    public float playerMaxHealth;
    public string currentScene;
    public string lastCheckpointId;
    
    [Header("Character Progression")]
    public ExperienceData experienceData;
    public PlayerStatsData statsData;
    
    [Header("Inventory Data")]
    public List<string> inventoryItems = new List<string>();
    public int currency;
    
    // Quest Data removed - placeholder for future implementation
    
    [Header("Game Progress")]
    public List<string> unlockedAreas = new List<string>();
    public List<string> discoveredCheckpoints = new List<string>();
    public float playtime;
    public int puzzleProgress;
    
    [Header("Settings")]
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool fullscreen = true;
    public int qualityLevel = 2;
    
    [Header("Statistics")]
    public int enemiesDefeated;
    public int itemsCollected;
    public int timesRevived;
    public DateTime lastSaveTime;
    
    public SaveData()
    {
        // Initialize with default values
        playerPosition = Vector3.zero;
        playerHealth = 100f;
        playerMaxHealth = 100f;
        currentScene = "MainMenu";
        lastCheckpointId = "";
        currency = 0;
        playtime = 0f;
        puzzleProgress = 0;
        enemiesDefeated = 0;
        itemsCollected = 0;
        timesRevived = 0;
        lastSaveTime = DateTime.Now;
        
        // Initialize progression data
        experienceData = new ExperienceData { level = 1, experience = 0, totalExperience = 0 };
        statsData = new PlayerStatsData 
        {
            level = 1,
            baseHealth = 100,
            baseMana = 50,
            baseAttack = 10,
            baseDefense = 5,
            baseSpeed = 8,
            baseLuck = 1,
            baseStamina = 50
        };
    }
}

[System.Serializable]
public class PlayerStatsData
{
    public int level;
    public float baseHealth;
    public float baseMana;
    public float baseAttack;
    public float baseDefense;
    public float baseSpeed;
    public float baseLuck;
    public float baseStamina;
    
    // Constructor with default values
    public PlayerStatsData()
    {
        level = 1;
        baseHealth = 100f;
        baseMana = 50f;
        baseAttack = 10f;
        baseDefense = 5f;
        baseSpeed = 8f;
        baseLuck = 1f;
        baseStamina = 50f;
    }
    
    // Constructor with specific values
    public PlayerStatsData(int level, float health, float mana, float attack, float defense, float speed, float luck, float stamina = 50f)
    {
        this.level = level;
        this.baseHealth = health;
        this.baseMana = mana;
        this.baseAttack = attack;
        this.baseDefense = defense;
        this.baseSpeed = speed;
        this.baseLuck = luck;
        this.baseStamina = stamina;
    }
}
