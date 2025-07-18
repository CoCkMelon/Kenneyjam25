using UnityEngine;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    public string savePointId;
    public bool isActivated = false;
    public bool autoSave = true;
    
    [Header("Visual Components")]
    public GameObject interactPrompt;
    public Animator savePointAnimator;
    public ParticleSystem saveEffect;
    public AudioSource saveSound;
    
    [Header("UI")]
    public GameObject saveUI;
    public Text saveStatusText;
    
    private bool playerInRange = false;
    private PlayerController playerController;
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Check if this save point was previously discovered
        SaveData saveData = SaveSystem.LoadGame();
        if (saveData.discoveredCheckpoints.Contains(savePointId))
        {
            ActivateSavePoint();
        }
        
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isActivated)
            {
                ActivateSavePoint();
            }
            else
            {
                SaveGameAtPoint();
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerController = other.GetComponent<PlayerController>();
            
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
                
            if (autoSave && isActivated)
            {
                SaveGameAtPoint();
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerController = null;
            
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
    
    void ActivateSavePoint()
    {
        if (isActivated) return;
        
        isActivated = true;
        
        // Visual feedback
        if (savePointAnimator != null)
            savePointAnimator.SetTrigger("Activate");
            
        if (saveEffect != null)
            saveEffect.Play();
            
        if (saveSound != null)
            saveSound.Play();
        
        // Add to discovered checkpoints
        SaveData saveData = SaveSystem.LoadGame();
        if (!saveData.discoveredCheckpoints.Contains(savePointId))
        {
            saveData.discoveredCheckpoints.Add(savePointId);
            SaveSystem.SaveGame(saveData);
        }
        
        // Show activation message
        ShowSaveMessage("Save Point Activated!");
        
        // Auto-save after activation
        if (autoSave)
        {
            SaveGameAtPoint();
        }
    }
    
    void SaveGameAtPoint()
    {
        if (!isActivated) return;
        
        SaveData saveData = SaveSystem.LoadGame();
        
        // Update player data
        if (playerController != null)
        {
            saveData.playerPosition = playerController.transform.position;
            saveData.playerHealth = playerController.GetComponent<Health>()?.CurrentHealth ?? 100f;
        }
        
        // Update checkpoint data
        saveData.lastCheckpointId = savePointId;
        saveData.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // Update game progress
        if (gameManager != null)
        {
            var inventory = gameManager.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                saveData.inventoryItems = inventory.GetInventoryItems();
                saveData.currency = inventory.GetCurrency();
            }
            
            // Quest system removed - placeholder for future implementation
        }
        
        // Update statistics
        saveData.lastSaveTime = System.DateTime.Now;
        
        // Save the game
        SaveSystem.SaveGame(saveData);
        
        // Visual feedback
        if (saveEffect != null)
            saveEffect.Play();
            
        if (saveSound != null)
            saveSound.Play();
        
        ShowSaveMessage("Game Saved!");
        
        Debug.Log($"Game saved at checkpoint: {savePointId}");
    }
    
    void ShowSaveMessage(string message)
    {
        if (saveUI != null && saveStatusText != null)
        {
            saveStatusText.text = message;
            saveUI.SetActive(true);
            
            // Hide message after 2 seconds
            Invoke("HideSaveMessage", 2f);
        }
    }
    
    void HideSaveMessage()
    {
        if (saveUI != null)
            saveUI.SetActive(false);
    }
    
    // Method to manually save game (can be called from UI)
    public void ManualSave()
    {
        SaveGameAtPoint();
    }
    
    // Method to load game from this save point
    public void LoadFromSavePoint()
    {
        SaveData saveData = SaveSystem.LoadGame();
        
        if (saveData.lastCheckpointId == savePointId)
        {
            // Load player to this position
            if (playerController != null)
            {
                playerController.transform.position = saveData.playerPosition;
                
                var health = playerController.GetComponent<Health>();
                if (health != null)
                {
                    health.SetHealth(saveData.playerHealth);
                }
            }
            
            ShowSaveMessage("Game Loaded!");
        }
    }
}
