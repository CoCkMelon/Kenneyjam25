using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public int score = 0;
    public int lives = 3;
    public float gameTime = 0f;
    public bool isGamePaused = false;
    
    [Header("Save System")]
    public bool autoSaveEnabled = true;
    public float autoSaveInterval = 300f; // 5 minutes
    private float lastAutoSaveTime = 0f;
    
    [Header("UI References")]
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI livesText;
    public TMPro.TextMeshProUGUI timeText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    
    [Header("Checkpoints")]
    public Transform[] checkpoints;
    public int currentCheckpoint = 0;
    
    private PlayerController player;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        UpdateUI();
    }
    
    void Update()
    {
        if (!isGamePaused)
        {
            gameTime += Time.deltaTime;
            UpdateUI();
            
            // Auto-save system
            if (autoSaveEnabled && Time.time - lastAutoSaveTime > autoSaveInterval)
            {
                AutoSave();
                lastAutoSaveTime = Time.time;
            }
        }
        
        // Pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Quick save (F5)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            QuickSave();
        }
        
        // Quick load (F9)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            QuickLoad();
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }
    
    public void LoseLife()
    {
        lives--;
        UpdateUI();
        
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnPlayer();
        }
    }
    
    public void RespawnPlayer()
    {
        if (player != null && checkpoints.Length > 0)
        {
            Vector3 spawnPos = checkpoints[currentCheckpoint].position;
            player.transform.position = spawnPos;
            
            // Reset player velocity
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
    
    public void SetCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex >= 0 && checkpointIndex < checkpoints.Length)
        {
            currentCheckpoint = checkpointIndex;
            Debug.Log($"Checkpoint {checkpointIndex} activated!");
        }
    }
    
    public void GameOver()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        Debug.Log("Game Over!");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(isGamePaused);
        }
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void NextLevel()
    {
        // Load next level (implement based on your level system)
        Debug.Log("Loading next level...");
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
            
        if (livesText != null)
            livesText.text = $"Lives: {lives}";
            
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    // Save System Methods
    public void AutoSave()
    {
        if (GameLoader.Instance != null)
        {
            GameLoader.Instance.QuickSave();
            Debug.Log("Auto-save completed!");
        }
    }
    
    public void QuickSave()
    {
        if (GameLoader.Instance != null)
        {
            GameLoader.Instance.QuickSave();
            Debug.Log("Quick save completed!");
        }
    }
    
    public void QuickLoad()
    {
        if (GameLoader.Instance != null)
        {
            GameLoader.Instance.LoadSavedGame();
            Debug.Log("Quick load completed!");
        }
    }
    
    public void SetPlaytime(float time)
    {
        gameTime = time;
    }
    
    public float GetPlaytime()
    {
        return gameTime;
    }
    
    public void SaveGameProgress()
    {
        SaveData saveData = SaveSystem.LoadGame();
        saveData.playtime = gameTime;
        saveData.currency = score; // Using score as currency for now
        SaveSystem.SaveGame(saveData);
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
