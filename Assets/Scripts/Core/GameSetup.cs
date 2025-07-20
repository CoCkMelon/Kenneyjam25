using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main game setup script that initializes and coordinates all systems
/// Place this on a GameObject in your main scene
/// </summary>
public class GameSetup : MonoBehaviour
{
    [Header("System Prefabs")]
    [SerializeField] private GameObject dialogueManagerPrefab;
    [SerializeField] private GameObject triggerManagerPrefab;
    [SerializeField] private GameObject storySequencerPrefab;
    [SerializeField] private GameObject cutsceneManagerPrefab;
    
    [Header("Puzzle Prefabs")]
    [SerializeField] private GameObject memoryPuzzlePrefab;
    [SerializeField] private GameObject powerFlowPuzzlePrefab;
    [SerializeField] private GameObject sleightGridPuzzlePrefab;
    
    [Header("Game Objects")]
    [SerializeField] private GameObject sleightControllerPrefab;
    [SerializeField] private GameObject powerOrbSpawnerPrefab;
    
    [Header("Initialization")]
    [SerializeField] private bool autoStartStory = true;
    [SerializeField] private bool createMissingManagers = true;
    
    [Header("Events")]
    public UnityEvent OnGameSetupComplete;
    public UnityEvent OnStorySystemsReady;
    public UnityEvent OnPuzzleSystemsReady;
    
    private void Start()
    {
        StartCoroutine(SetupGameSystems());
    }
    
    private System.Collections.IEnumerator SetupGameSystems()
    {
        Debug.Log("Starting game setup...");
        
        // Setup core managers
        yield return StartCoroutine(SetupCoreManagers());
        
        // Setup story systems
        yield return StartCoroutine(SetupStorySystem());
        
        // Setup puzzle systems
        yield return StartCoroutine(SetupPuzzleSystem());
        
        // Setup game objects
        yield return StartCoroutine(SetupGameObjects());
        
        // Connect all systems
        ConnectSystems();
        
        // Complete setup
        CompleteSetup();
        
        yield return null;
    }
    
    private System.Collections.IEnumerator SetupCoreManagers()
    {
        Debug.Log("Setting up core managers...");
        
        // Ensure TriggerManager exists
        if (TriggerManager.Instance == null && createMissingManagers)
        {
            if (triggerManagerPrefab != null)
            {
                Instantiate(triggerManagerPrefab);
            }
            else
            {
                new GameObject("TriggerManager").AddComponent<TriggerManager>();
            }
        }
        
        // Ensure DialogueManager exists
        if (DialogueManager.Instance == null && createMissingManagers)
        {
            if (dialogueManagerPrefab != null)
            {
                Instantiate(dialogueManagerPrefab);
            }
            else
            {
                new GameObject("DialogueManager").AddComponent<DialogueManager>();
            }
        }
        
        yield return new WaitForEndOfFrame();
    }
    
    private System.Collections.IEnumerator SetupStorySystem()
    {
        Debug.Log("Setting up story system...");
        
        // Ensure StorySequencer exists
        if (StorySequencer.Instance == null && createMissingManagers)
        {
            if (storySequencerPrefab != null)
            {
                Instantiate(storySequencerPrefab);
            }
            else
            {
                new GameObject("StorySequencer").AddComponent<StorySequencer>();
            }
        }
        
        // Ensure CutsceneManager exists
        if (FindFirstObjectByType<CutsceneManager>() == null && createMissingManagers)
        {
            if (cutsceneManagerPrefab != null)
            {
                Instantiate(cutsceneManagerPrefab);
            }
            else
            {
                new GameObject("CutsceneManager").AddComponent<CutsceneManager>();
            }
        }
        
        OnStorySystemsReady?.Invoke();
        yield return new WaitForEndOfFrame();
    }
    
    private System.Collections.IEnumerator SetupPuzzleSystem()
    {
        Debug.Log("Setting up puzzle system...");
        
        // Create puzzle objects (initially disabled)
        CreatePuzzleObject<MemoryPatternPuzzle>(memoryPuzzlePrefab, "MemoryPuzzle", false);
        CreatePuzzleObject<PowerFlowPuzzleSingleFile>(powerFlowPuzzlePrefab, "PowerFlowPuzzle", false);
        CreatePuzzleObject<SleightGridPuzzle>(sleightGridPuzzlePrefab, "SleightGridPuzzle", false);
        
        OnPuzzleSystemsReady?.Invoke();
        yield return new WaitForEndOfFrame();
    }
    
    private void CreatePuzzleObject<T>(GameObject prefab, string objectName, bool startActive) where T : MonoBehaviour
    {
        if (FindFirstObjectByType<T>() == null)
        {
            GameObject puzzleObj;
            if (prefab != null)
            {
                puzzleObj = Instantiate(prefab);
            }
            else
            {
                puzzleObj = new GameObject(objectName);
                puzzleObj.AddComponent<T>();
            }
            
            puzzleObj.SetActive(startActive);
        }
    }
    
    private System.Collections.IEnumerator SetupGameObjects()
    {
        Debug.Log("Setting up game objects...");
        
        // Ensure SleightController exists
        if (FindFirstObjectByType<SleightController>() == null)
        {
            if (sleightControllerPrefab != null)
            {
                Instantiate(sleightControllerPrefab);
            }
            else
            {
                Debug.LogWarning("SleightController prefab not assigned and none found in scene!");
            }
        }
        
        // Ensure PowerOrbSpawner exists (initially disabled)
        if (FindFirstObjectByType<PowerOrbSpawner>() == null)
        {
            GameObject spawnerObj;
            if (powerOrbSpawnerPrefab != null)
            {
                spawnerObj = Instantiate(powerOrbSpawnerPrefab);
            }
            else
            {
                spawnerObj = new GameObject("PowerOrbSpawner");
                spawnerObj.AddComponent<PowerOrbSpawner>();
            }
            
            // Start disabled until story enables it
            spawnerObj.SetActive(false);
        }
        
        yield return new WaitForEndOfFrame();
    }
    
    private void ConnectSystems()
    {
        Debug.Log("Connecting systems...");
        
        // Connect TriggerManager events if needed
        var triggerManager = TriggerManager.Instance;
        if (triggerManager != null)
        {
            // Systems are auto-connected through their Start methods
            Debug.Log("TriggerManager connected to other systems");
        }
        
        // Connect StorySequencer events
        var storySequencer = StorySequencer.Instance;
        if (storySequencer != null)
        {
            // Additional event connections can go here
            Debug.Log("StorySequencer ready for story progression");
        }
    }
    
    private void CompleteSetup()
    {
        Debug.Log("Game setup complete!");
        OnGameSetupComplete?.Invoke();
        
        // Start the story if configured to do so
        if (autoStartStory && StorySequencer.Instance != null)
        {
            // Small delay to ensure all systems are ready
            Invoke(nameof(StartStoryDelayed), 0.5f);
        }
    }
    
    private void StartStoryDelayed()
    {
        if (StorySequencer.Instance != null)
        {
            StorySequencer.Instance.StartStory();
        }
    }
    
    // Debug methods
    [ContextMenu("Force Setup")]
    public void ForceSetup()
    {
        StartCoroutine(SetupGameSystems());
    }
    
    [ContextMenu("Start Story")]
    public void StartStoryManual()
    {
        if (StorySequencer.Instance != null)
        {
            StorySequencer.Instance.StartStory();
        }
    }
}
