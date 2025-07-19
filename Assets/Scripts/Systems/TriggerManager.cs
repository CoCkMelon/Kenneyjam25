using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Enhanced TriggerManager that handles story progression, puzzle transitions, and game logic
/// </summary>
public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;
    
    [Header("System References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private SleightController sleightController;
    
    [Header("Puzzle References")]
    [SerializeField] private MemoryPatternPuzzle memoryPuzzle;
    [SerializeField] private PowerFlowPuzzleSingle powerFlowPuzzle;
    [SerializeField] private SleightGridPuzzle sleightGridPuzzle;
    
    [Header("Story State")]
    [SerializeField] private bool hasCollectedFirstOrb = false;
    [SerializeField] private bool memoryPuzzleCompleted = false;
    [SerializeField] private bool powerFlowPuzzleCompleted = false;
    [SerializeField] private bool sleightGridPuzzleCompleted = false;
    [SerializeField] private bool isLightSpeedAchieved = false;
    
    [Header("Events")]
    public UnityEvent OnOrbCollectionEnabled;
    public UnityEvent OnLightSpeedAchieved;
    public UnityEvent OnAllPuzzlesComplete;
    public UnityEvent OnStoryComplete;
    
    // Story progression tracking
    private int currentStorySegment = 0;
    private bool isPuzzleMode = false;
    
    private void Awake() 
    {
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
        // Find system references if not assigned
        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (cutsceneManager == null)
            cutsceneManager = FindFirstObjectByType<CutsceneManager>();
        if (sleightController == null)
            sleightController = FindFirstObjectByType<SleightController>();
            
        // Find puzzle references
        if (memoryPuzzle == null)
            memoryPuzzle = FindFirstObjectByType<MemoryPatternPuzzle>();
        if (powerFlowPuzzle == null)
            powerFlowPuzzle = FindFirstObjectByType<PowerFlowPuzzleSingle>();
        if (sleightGridPuzzle == null)
            sleightGridPuzzle = FindFirstObjectByType<SleightGridPuzzle>();
            
        SetupPuzzleEvents();
    }
    
    private void SetupPuzzleEvents()
    {
        // Setup puzzle completion events
        if (memoryPuzzle != null)
            memoryPuzzle.OnPuzzleComplete.AddListener(OnMemoryPuzzleComplete);
        if (powerFlowPuzzle != null)
            powerFlowPuzzle.OnPuzzleComplete.AddListener(OnPowerFlowPuzzleComplete);
        if (sleightGridPuzzle != null)
            sleightGridPuzzle.OnPuzzleSolved.AddListener(OnSleightGridPuzzleComplete);
    }

    public void Trigger(string triggerName, DialogueLine line)
    {
        Debug.Log($"Trigger activated: {triggerName}");
        
        switch (triggerName)
        {
            // Story triggers
            case "enable_orb_collection":
                EnableOrbCollection();
                break;
            case "unlock_speed_research":
                UnlockSpeedResearch();
                break;
            case "prepare_light_speed_sequence":
                PrepareLightSpeedSequence();
                break;
            case "transcendence_complete":
                TranscendenceComplete();
                break;
                
            // Puzzle triggers
            case "start_memory_puzzle":
                StartMemoryPuzzle();
                break;
            case "start_power_flow_puzzle":
                StartPowerFlowPuzzle();
                break;
            case "start_sleight_grid_puzzle":
                StartSleightGridPuzzle();
                break;
                
            // Game state triggers
            case "unlock_free_play_mode":
                UnlockFreePlayMode();
                break;
            case "begin_main_gameplay":
                BeginMainGameplay();
                break;
                
            // Value system triggers (from StoryRouteManager)
            case "award_honesty":
                AwardValue("Honesty", 10);
                break;
            case "award_humility":
                AwardValue("Humility", 10);
                break;
            case "award_compassion":
                AwardValue("Compassion", 15);
                break;
            case "award_openness":
                AwardValue("Empathy", 10);
                break;
            case "award_rationality":
                AwardValue("Logic", 10);
                break;
            case "award_altruism":
                AwardValue("Compassion", 15);
                break;
            case "award_ambition":
                AwardValue("Responsibility", -5); // Negative for selfish choices
                break;
                
            // Scene transitions
            case "scene_transition":
                if(line != null && !string.IsNullOrEmpty(line.next_scene))
                    LoadDialogueScene(line.next_scene);
                break;
                
            // Legacy triggers
            case "start_quest":
                Debug.Log("Quest system not implemented yet");
                break;
                
            default:
                Debug.LogWarning($"Unknown trigger: {triggerName}");
                break;
        }
    }
    
    #region Story Progression
    
    private void EnableOrbCollection()
    {
        hasCollectedFirstOrb = true;
        OnOrbCollectionEnabled?.Invoke();
        
        // Enable power orb spawning system
        var orbSpawner = FindFirstObjectByType<PowerOrbSpawner>();
        if (orbSpawner != null)
        {
            orbSpawner.enabled = true;
        }
        
        Debug.Log("Orb collection enabled! The sled can now gather energy.");
    }
    
    private void UnlockSpeedResearch()
    {
        // Enable advanced sled capabilities
        if (sleightController != null)
        {
            // Unlock higher speed tiers
            Debug.Log("Speed research unlocked! Sled capabilities enhanced.");
        }
    }
    
    private void PrepareLightSpeedSequence()
    {
        if (cutsceneManager != null)
        {
            cutsceneManager.PlayLightSpeedCutscene();
        }
        
        isLightSpeedAchieved = true;
        OnLightSpeedAchieved?.Invoke();
        
        // Start the puzzle sequence after a delay
        StartCoroutine(DelayedPuzzleStart());
    }
    
    private IEnumerator DelayedPuzzleStart()
    {
        yield return new WaitForSeconds(2f);
        LoadDialogueScene("puzzle_transition");

    }
    
    private void TranscendenceComplete()
    {
        Debug.Log("Transcendence achieved! The sled has mastered faster-than-light travel.");
    }
    
    #endregion
    
    #region Puzzle Management
    
    private void StartMemoryPuzzle()
    {
        isPuzzleMode = true;
        
        if (memoryPuzzle != null)
        {
            memoryPuzzle.gameObject.SetActive(true);
            memoryPuzzle.StartPuzzle();
        }
        else
        {
            Debug.LogWarning("Memory puzzle not found! Skipping to next scene.");
            OnMemoryPuzzleComplete();
        }
    }
    
    private void StartPowerFlowPuzzle()
    {
        isPuzzleMode = true;
        
        if (powerFlowPuzzle != null)
        {
            powerFlowPuzzle.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Power flow puzzle not found! Skipping to next scene.");
            OnPowerFlowPuzzleComplete();
        }
    }
    
    private void StartSleightGridPuzzle()
    {
        isPuzzleMode = true;
        
        if (sleightGridPuzzle != null)
        {
            sleightGridPuzzle.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Sleight grid puzzle not found! Skipping to next scene.");
            OnSleightGridPuzzleComplete();
        }
    }
    
    private void OnMemoryPuzzleComplete()
    {
        memoryPuzzleCompleted = true;
        isPuzzleMode = false;
        
        if (memoryPuzzle != null)
            memoryPuzzle.gameObject.SetActive(false);
            
        Debug.Log("Memory puzzle completed! Moving to next story segment.");
        LoadDialogueScene("memory_puzzle_complete");
    }
    
    private void OnPowerFlowPuzzleComplete()
    {
        powerFlowPuzzleCompleted = true;
        isPuzzleMode = false;
        
        if (powerFlowPuzzle != null)
            powerFlowPuzzle.gameObject.SetActive(false);
            
        Debug.Log("Power flow puzzle completed! Moving to next story segment.");
        LoadDialogueScene("power_flow_complete");
    }
    
    private void OnSleightGridPuzzleComplete()
    {
        sleightGridPuzzleCompleted = true;
        isPuzzleMode = false;
        
        if (sleightGridPuzzle != null)
            sleightGridPuzzle.gameObject.SetActive(false);
            
        Debug.Log("Sleight grid puzzle completed! All puzzles solved!");
        OnAllPuzzlesComplete?.Invoke();
        LoadDialogueScene("puzzle_victory");
    }
    
    #endregion
    
    #region Game State Management
    
    private void UnlockFreePlayMode()
    {
        Debug.Log("Free play mode unlocked! The sled is fully mastered.");
        
        // Enable all sled capabilities
        if (sleightController != null)
        {
            // Unlock maximum speed and all abilities
        }
        
        OnStoryComplete?.Invoke();
    }
    
    private void BeginMainGameplay()
    {
        Debug.Log("Main gameplay begins! The adventure awaits.");
        
        // Enable main game systems
        // Hide dialogue UI
        if (dialogueManager != null)
        {
            // Switch to gameplay mode
        }
    }
    
    private void LoadDialogueScene(string sceneName)
    {
        if (dialogueManager != null)
        {
            string scenePath = $"StoryRoutes/{sceneName}";
            dialogueManager.LoadAndStartSceneFromResources(scenePath);
        }
        else
        {
            Debug.LogError("DialogueManager not found! Cannot load scene: " + sceneName);
        }
    }
    
    private void AwardValue(string valueName, int points)
    {
        // Integration with StoryRouteManager if available
        var storyManager = FindFirstObjectByType<StoryRouteManager>();
        if (storyManager != null)
        {
            // Award value points (would need to extend StoryRouteManager)
            Debug.Log($"Awarded {points} {valueName} points");
        }
    }
    
    #endregion
    
    #region Public Properties
    
    public bool HasCollectedFirstOrb => hasCollectedFirstOrb;
    public bool IsMemoryPuzzleCompleted => memoryPuzzleCompleted;
    public bool IsPowerFlowPuzzleCompleted => powerFlowPuzzleCompleted;
    public bool IsSleightGridPuzzleCompleted => sleightGridPuzzleCompleted;
    public bool IsLightSpeedAchieved => isLightSpeedAchieved;
    public bool IsInPuzzleMode => isPuzzleMode;
    public bool AreAllPuzzlesCompleted => memoryPuzzleCompleted && powerFlowPuzzleCompleted && sleightGridPuzzleCompleted;
    
    #endregion
}

