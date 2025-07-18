using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    [Header("Loading Settings")]
    public string defaultScene = "Forest";
    public Vector3 defaultSpawnPoint = Vector3.zero;
    
    private static GameLoader instance;
    public static GameLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameLoader>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameLoader");
                    instance = go.AddComponent<GameLoader>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadSavedGame()
    {
        SaveData saveData = SaveSystem.LoadGame();
        
        if (saveData != null && !string.IsNullOrEmpty(saveData.currentScene))
        {
            StartCoroutine(LoadGameCoroutine(saveData));
        }
        else
        {
            Debug.LogWarning("No save data found or invalid scene. Starting new game.");
            StartNewGame();
        }
    }
    
    public void StartNewGame()
    {
        // Create new save data
        SaveData newSave = new SaveData();
        newSave.currentScene = defaultScene;
        newSave.playerPosition = defaultSpawnPoint;
        newSave.playerHealth = 100f;
        newSave.playerMaxHealth = 100f;
        
        SaveSystem.SaveGame(newSave);
        
        // Load the default scene
        SceneManager.LoadScene(defaultScene);
    }
    
    IEnumerator LoadGameCoroutine(SaveData saveData)
    {
        // Load the saved scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(saveData.currentScene);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Wait for one frame to ensure scene is fully loaded
        yield return null;
        
        // Restore player state
        RestorePlayerState(saveData);
        
        // Restore game state
        RestoreGameState(saveData);
    }
    
    void RestorePlayerState(SaveData saveData)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Restore position
            player.transform.position = saveData.playerPosition;
            
            // Restore health
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.SetMaxHealth(saveData.playerMaxHealth);
                playerHealth.SetHealth(saveData.playerHealth);
            }
            
            // Restore inventory
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.LoadInventory(saveData.inventoryItems);
                inventory.SetCurrency(saveData.currency);
            }
            
            // Restore experience
            ExperienceSystem experienceSystem = player.GetComponent<ExperienceSystem>();
            if (experienceSystem != null && saveData.experienceData != null)
            {
                experienceSystem.LoadExperienceData(saveData.experienceData);
            }
            
            // Restore stats
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null && saveData.statsData != null)
            {
                playerStats.LoadStatsData(saveData.statsData);
            }
        }
        else
        {
            Debug.LogError("Player not found in scene!");
        }
    }
    
    void RestoreGameState(SaveData saveData)
    {
        // Quest system removed - placeholder for future implementation
        
        // Restore discovered checkpoints
        SavePoint[] savePoints = FindObjectsByType<SavePoint>(FindObjectsSortMode.None);
        foreach (SavePoint savePoint in savePoints)
        {
            if (saveData.discoveredCheckpoints.Contains(savePoint.savePointId))
            {
                // Activate discovered save points
                savePoint.isActivated = true;
            }
        }
        
        // Restore game settings
        RestoreSettings(saveData);
        
        // Update playtime tracking
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SetPlaytime(saveData.playtime);
        }
    }
    
    void RestoreSettings(SaveData saveData)
    {
        // Restore audio settings
        AudioListener.volume = saveData.masterVolume;
        
        // Restore graphics settings
        QualitySettings.SetQualityLevel(saveData.qualityLevel);
        
        // Restore display settings
        if (saveData.fullscreen != Screen.fullScreen)
        {
            Screen.fullScreen = saveData.fullscreen;
        }
    }
    
    public void QuickSave()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SaveData saveData = SaveSystem.LoadGame();
            
            // Update current state
            saveData.playerPosition = player.transform.position;
            saveData.currentScene = SceneManager.GetActiveScene().name;
            
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                saveData.playerHealth = playerHealth.CurrentHealth;
                saveData.playerMaxHealth = playerHealth.MaxHealth;
            }
            
            // Update inventory
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                saveData.inventoryItems = inventory.GetInventoryItems();
                saveData.currency = inventory.GetCurrency();
            }
            
            // Update experience
            ExperienceSystem experienceSystem = player.GetComponent<ExperienceSystem>();
            if (experienceSystem != null)
            {
                saveData.experienceData = experienceSystem.GetExperienceData();
            }
            
            // Update stats
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                saveData.statsData = playerStats.GetStatsData();
            }
            
            // Quest system removed - placeholder for future implementation
            
            // Update playtime
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                saveData.playtime = gameManager.GetPlaytime();
            }
            
            saveData.lastSaveTime = System.DateTime.Now;
            
            SaveSystem.SaveGame(saveData);
            
            Debug.Log("Quick save completed!");
        }
    }
    
    public void RespawnAtLastCheckpoint()
    {
        SaveData saveData = SaveSystem.LoadGame();
        
        if (!string.IsNullOrEmpty(saveData.lastCheckpointId))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = saveData.playerPosition;
                
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.SetHealth(saveData.playerHealth);
                    playerHealth.OnRespawn();
                }
                
                // Increment revival count
                saveData.timesRevived++;
                SaveSystem.SaveGame(saveData);
                
                Debug.Log($"Player respawned at checkpoint: {saveData.lastCheckpointId}");
            }
        }
        else
        {
            Debug.LogWarning("No checkpoint found for respawn!");
        }
    }
    
    public bool HasSaveData()
    {
        SaveData saveData = SaveSystem.LoadGame();
        return saveData != null && !string.IsNullOrEmpty(saveData.currentScene);
    }
    
    public void DeleteSaveData()
    {
        SaveSystem.DeleteSaveFile();
        Debug.Log("Save data deleted.");
    }
}
