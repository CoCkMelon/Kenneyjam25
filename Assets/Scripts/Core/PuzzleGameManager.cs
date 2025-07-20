using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Puzzle Game Manager - Manages puzzle-specific game flow and state
/// Handles multiple puzzles, progress tracking, and UI coordination
/// </summary>
public class PuzzleGameManager : MonoBehaviour
{
    public static PuzzleGameManager Instance { get; private set; }
    
    [Header("Puzzle Configuration")]
    [SerializeField] private List<GameObject> puzzlePrefabs = new List<GameObject>();
    [SerializeField] private Transform puzzleContainer;
    [SerializeField] private int currentPuzzleIndex = 0;
    [SerializeField] private bool randomizePuzzleOrder = false;
    
    [Header("Game State")]
    [SerializeField] private int totalPuzzlesSolved = 0;
    [SerializeField] private float totalGameTime = 0f;
    [SerializeField] private int hintsUsed = 0;
    [SerializeField] private int maxHintsAllowed = 3;
    [SerializeField] private bool isPaused = false;
    
    [Header("Scoring System")]
    [SerializeField] private int baseScorePerPuzzle = 100;
    [SerializeField] private int timeBonus = 50;
    [SerializeField] private int hintPenalty = 25;
    [SerializeField] private int currentScore = 0;
    
    [Header("UI References")]
    [SerializeField] private PuzzleHUDController hudController;
    [SerializeField] private GameObject pauseMenu;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip puzzleCompleteSound;
    [SerializeField] private AudioClip gameCompleteSound;
    [SerializeField] private AudioClip hintSound;
    
    // Current puzzle state
    private GameObject currentPuzzleInstance;
    private MonoBehaviour currentPuzzleComponent;
    private float currentPuzzleStartTime;
    private bool isTransitioningPuzzles = false;
    
    // Puzzle progress tracking
    private Dictionary<int, float> puzzleCompletionTimes = new Dictionary<int, float>();
    private List<int> puzzleOrder = new List<int>();
    
    // Events
    public System.Action<int> OnPuzzleCompleted;
    public System.Action OnAllPuzzlesCompleted;
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnHintUsed;
    
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
            return;
        }
        
        InitializeAudio();
    }
    
    void Start()
    {
        InitializePuzzleOrder();
        StartFirstPuzzle();
        
        if (hudController == null)
            hudController = FindFirstObjectByType<PuzzleHUDController>();
    }
    
    void Update()
    {
        if (!isPaused && !isTransitioningPuzzles)
        {
            totalGameTime += Time.deltaTime;
            
            // Update HUD if available
            if (hudController != null)
            {
                hudController.UpdateGameTime(totalGameTime);
                hudController.UpdateCurrentPuzzleTime(Time.time - currentPuzzleStartTime);
            }
        }
        
        HandleInput();
    }
    
    private void InitializeAudio()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    
    private void InitializePuzzleOrder()
    {
        puzzleOrder.Clear();
        
        for (int i = 0; i < puzzlePrefabs.Count; i++)
        {
            puzzleOrder.Add(i);
        }
        
        if (randomizePuzzleOrder)
        {
            // Shuffle the puzzle order
            for (int i = 0; i < puzzleOrder.Count; i++)
            {
                int randomIndex = Random.Range(i, puzzleOrder.Count);
                int temp = puzzleOrder[i];
                puzzleOrder[i] = puzzleOrder[randomIndex];
                puzzleOrder[randomIndex] = temp;
            }
        }
        
        currentPuzzleIndex = 0;
    }
    
    private void HandleInput()
    {
        // Pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Hint request
        if (Input.GetKeyDown(KeyCode.H))
        {
            RequestHint();
        }
        
        // Skip puzzle (for debugging)
        if (Input.GetKeyDown(KeyCode.N) && Debug.isDebugBuild)
        {
            SkipCurrentPuzzle();
        }
        
        // Reset puzzle
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCurrentPuzzle();
        }
    }
    
    private void StartFirstPuzzle()
    {
        if (puzzlePrefabs.Count > 0)
        {
            LoadPuzzle(0);
        }
        else
        {
            Debug.LogError("No puzzle prefabs configured!");
        }
    }
    
    public void LoadPuzzle(int puzzleOrderIndex)
    {
        if (isTransitioningPuzzles) return;
        
        StartCoroutine(LoadPuzzleCoroutine(puzzleOrderIndex));
    }
    
    private IEnumerator LoadPuzzleCoroutine(int puzzleOrderIndex)
    {
        isTransitioningPuzzles = true;
        
        // Clear current puzzle
        if (currentPuzzleInstance != null)
        {
            Destroy(currentPuzzleInstance);
            yield return null; // Wait one frame
        }
        
        // Load new puzzle
        if (puzzleOrderIndex < puzzleOrder.Count)
        {
            int actualPuzzleIndex = puzzleOrder[puzzleOrderIndex];
            GameObject puzzlePrefab = puzzlePrefabs[actualPuzzleIndex];
            
            currentPuzzleInstance = Instantiate(puzzlePrefab, puzzleContainer);
            currentPuzzleIndex = puzzleOrderIndex;
            currentPuzzleStartTime = Time.time;
            
            // Get puzzle component and set up events
            SetupPuzzleEvents();
            
            // Update HUD
            if (hudController != null)
            {
                hudController.OnPuzzleLoaded(puzzleOrderIndex + 1, puzzleOrder.Count, GetPuzzleName(actualPuzzleIndex));
            }
            
            Debug.Log($"Loaded puzzle {puzzleOrderIndex + 1}/{puzzleOrder.Count}: {GetPuzzleName(actualPuzzleIndex)}");
        }
        
        isTransitioningPuzzles = false;
    }
    
    private void SetupPuzzleEvents()
    {
        // Try to find different types of puzzle components and hook up their events
        
        // Memory Pattern Puzzle
        var memoryPuzzle = currentPuzzleInstance.GetComponent<MemoryPatternPuzzle>();
        if (memoryPuzzle != null)
        {
            currentPuzzleComponent = memoryPuzzle;
            memoryPuzzle.OnPuzzleComplete.AddListener(OnCurrentPuzzleCompleted);
            return;
        }
        
        // Sleight Grid Puzzle
        var gridPuzzle = currentPuzzleInstance.GetComponent<SleightGridPuzzle>();
        if (gridPuzzle != null)
        {
            currentPuzzleComponent = gridPuzzle;
            gridPuzzle.OnPuzzleSolved.AddListener(OnCurrentPuzzleCompleted);
            return;
        }
        
        // Power Flow Puzzle
        var powerPuzzle = currentPuzzleInstance.GetComponent<PowerFlowPuzzleSingleFile>();
        if (powerPuzzle != null)
        {
            currentPuzzleComponent = powerPuzzle;
            // Power flow puzzle doesn't have built-in events, so we'll need to poll or modify it
            StartCoroutine(MonitorPowerFlowPuzzle(powerPuzzle));
            return;
        }
        
        Debug.LogWarning("No recognized puzzle component found on loaded puzzle!");
    }
    
    private IEnumerator MonitorPowerFlowPuzzle(PowerFlowPuzzleSingleFile powerPuzzle)
    {
        bool wasCompleted = false;
        
        while (currentPuzzleComponent == powerPuzzle && currentPuzzleInstance != null)
        {
            bool isCompleted = powerPuzzle.IsPuzzleComplete;
            
            if (isCompleted && !wasCompleted)
            {
                OnCurrentPuzzleCompleted();
                break;
            }
            
            wasCompleted = isCompleted;
            yield return new WaitForSeconds(0.1f); // Check every 100ms
        }
    }
    
    private void OnCurrentPuzzleCompleted()
    {
        if (isTransitioningPuzzles) return;
        
        float completionTime = Time.time - currentPuzzleStartTime;
        puzzleCompletionTimes[currentPuzzleIndex] = completionTime;
        
        totalPuzzlesSolved++;
        
        // Calculate score for this puzzle
        int puzzleScore = CalculatePuzzleScore(completionTime);
        currentScore += puzzleScore;
        
        // Play completion sound
        if (audioSource != null && puzzleCompleteSound != null)
            audioSource.PlayOneShot(puzzleCompleteSound);
        
        // Trigger events
        OnPuzzleCompleted?.Invoke(currentPuzzleIndex);
        OnScoreChanged?.Invoke(currentScore);
        
        // Update HUD
        if (hudController != null)
        {
            hudController.OnPuzzleCompleted(currentPuzzleIndex + 1, completionTime, puzzleScore);
        }
        
        Debug.Log($"Puzzle {currentPuzzleIndex + 1} completed in {completionTime:F2} seconds! Score: +{puzzleScore}");
        
        // Check if all puzzles are complete
        if (currentPuzzleIndex >= puzzleOrder.Count - 1)
        {
            StartCoroutine(CompleteAllPuzzles());
        }
        else
        {
            StartCoroutine(TransitionToNextPuzzle());
        }
    }
    
    private IEnumerator TransitionToNextPuzzle()
    {
        yield return new WaitForSeconds(2f); // Show completion for 2 seconds
        LoadPuzzle(currentPuzzleIndex + 1);
    }
    
    private IEnumerator CompleteAllPuzzles()
    {
        yield return new WaitForSeconds(2f);
        
        // Play game complete sound
        if (audioSource != null && gameCompleteSound != null)
            audioSource.PlayOneShot(gameCompleteSound);
        
        // Update HUD to show completion
        if (hudController != null)
        {
            hudController.OnAllPuzzlesCompleted(totalGameTime, currentScore);
        }
        
        OnAllPuzzlesCompleted?.Invoke();
        
        Debug.Log($"All puzzles completed! Total time: {totalGameTime:F2}s, Final score: {currentScore}");
    }
    
    private int CalculatePuzzleScore(float completionTime)
    {
        int score = baseScorePerPuzzle;
        
        // Time bonus (faster completion = higher bonus)
        if (completionTime < 30f)
            score += timeBonus;
        else if (completionTime < 60f)
            score += timeBonus / 2;
        
        // Hint penalty
        score -= hintsUsed * hintPenalty;
        
        return Mathf.Max(score, 10); // Minimum 10 points
    }
    
    public void RequestHint()
    {
        if (hintsUsed >= maxHintsAllowed || isPaused || isTransitioningPuzzles)
            return;
        
        hintsUsed++;
        
        // Play hint sound
        if (audioSource != null && hintSound != null)
            audioSource.PlayOneShot(hintSound);
        
        // Show hint through HUD controller
        string hintMessage = GetPuzzleHint();
        if (hudController != null)
        {
            hudController.ShowHint(hintMessage);
        }
        
        // Update HUD
        if (hudController != null)
        {
            hudController.OnHintUsed(hintsUsed);
        }
        
        OnHintUsed?.Invoke(hintsUsed);
        
        Debug.Log($"Hint used ({hintsUsed}/{maxHintsAllowed})");
    }
    
    private string GetPuzzleHint()
    {
        // Return specific hints based on current puzzle type
        if (currentPuzzleComponent is MemoryPatternPuzzle)
        {
            if (hintsUsed == 1)
                return "Watch the pattern more carefully!";
            else if (hintsUsed == 2)
                return "Try to find a rhythm or mnemonic!";
            else
                return "Focus on the sequence order!";
        }
        else if (currentPuzzleComponent is SleightGridPuzzle gridPuzzle)
        {
            if (hintsUsed == 1)
                return "Check row and column constraints!";
            else if (hintsUsed == 2)
            {
                gridPuzzle.ShowSolution(); // Show/hide solution
                return "Solution highlighted!";
            }
            else
                return "Look for patterns in the grid!";
        }
        else if (currentPuzzleComponent is PowerFlowPuzzleSingleFile)
        {
            return "Follow the power flow from source to destination!";
        }
        
        return "Think outside the box!";
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (pauseMenu != null)
            pauseMenu.SetActive(isPaused);
        
        if (hudController != null)
            hudController.OnGamePausedChanged(isPaused);
        
        Debug.Log($"Game {(isPaused ? "paused" : "resumed")}");
    }
    
    public void ResetCurrentPuzzle()
    {
        if (currentPuzzleComponent == null || isTransitioningPuzzles) return;
        
        // Reset puzzle-specific state
        if (currentPuzzleComponent is MemoryPatternPuzzle memoryPuzzle)
        {
            memoryPuzzle.ResetPuzzle();
        }
        else if (currentPuzzleComponent is SleightGridPuzzle gridPuzzle)
        {
            gridPuzzle.ResetPuzzle();
        }
        // Power flow puzzle doesn't have a reset method, would need to reload
        
        currentPuzzleStartTime = Time.time;
        
        Debug.Log("Current puzzle reset");
    }
    
    public void SkipCurrentPuzzle()
    {
        if (!Debug.isDebugBuild) return;
        
        Debug.Log("Skipping current puzzle (debug mode)");
        OnCurrentPuzzleCompleted();
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    private string GetPuzzleName(int puzzleIndex)
    {
        if (puzzleIndex < puzzlePrefabs.Count)
        {
            return puzzlePrefabs[puzzleIndex].name;
        }
        return $"Puzzle {puzzleIndex + 1}";
    }
    
    // Public getters for external systems
    public int CurrentPuzzleIndex => currentPuzzleIndex + 1; // 1-based for display
    public int TotalPuzzles => puzzleOrder.Count;
    public int PuzzlesSolved => totalPuzzlesSolved;
    public float TotalGameTime => totalGameTime;
    public int CurrentScore => currentScore;
    public int HintsUsed => hintsUsed;
    public int MaxHintsAllowed => maxHintsAllowed;
    public bool IsPaused => isPaused;
    public bool IsTransitioning => isTransitioningPuzzles;
    
    // Save system integration
    public void SaveProgress()
    {
        // Save puzzle progress to the existing save system
        SaveData saveData = SaveSystem.LoadGame();
        saveData.puzzleProgress = currentPuzzleIndex;
        saveData.currency = currentScore;
        saveData.playtime = totalGameTime;
        SaveSystem.SaveGame(saveData);
    }
    
    public void LoadProgress()
    {
        SaveData saveData = SaveSystem.LoadGame();
        if (saveData != null)
        {
            currentScore = saveData.currency;
            totalGameTime = saveData.playtime;
            
            if (saveData.puzzleProgress > 0 && saveData.puzzleProgress < puzzleOrder.Count)
            {
                LoadPuzzle(saveData.puzzleProgress);
            }
        }
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
