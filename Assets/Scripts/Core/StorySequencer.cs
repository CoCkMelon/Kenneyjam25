using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the overall story sequence and provides easy story progression
/// </summary>
public class StorySequencer : MonoBehaviour
{
    public static StorySequencer Instance;
    
    [Header("Story Scenes")]
    [SerializeField] private string[] storyScenes = {
        "01_city_streets_discovery",
        "02_workshop_investigation", 
        "03_sled_ascension",
        "04_faster_than_light",
        "05_puzzle_transition",
        "06_memory_puzzle_complete",
        "07_power_flow_complete",
        "08_puzzle_victory"
    };
    
    [Header("Current Progress")]
    [SerializeField] private int currentSceneIndex = 0;
    [SerializeField] private bool autoProgressStory = true;
    
    [Header("Events")]
    public UnityEvent OnStoryStart;
    public UnityEvent OnStoryComplete;
    public UnityEvent OnSceneTransition;
    
    private DialogueManager dialogueManager;
    private TriggerManager triggerManager;
    
    void Awake()
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
        dialogueManager = DialogueManager.Instance;
        triggerManager = TriggerManager.Instance;
        
        // Start the story automatically
        if (autoProgressStory)
        {
            StartStory();
        }
    }
    
    public void StartStory()
    {
        currentSceneIndex = 0;
        OnStoryStart?.Invoke();
        LoadCurrentScene();
    }
    
    public void AdvanceToNextScene()
    {
        if (currentSceneIndex < storyScenes.Length - 1)
        {
            currentSceneIndex++;
            OnSceneTransition?.Invoke();
            LoadCurrentScene();
        }
        else
        {
            CompleteStory();
        }
    }
    
    public void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < storyScenes.Length)
        {
            currentSceneIndex = index;
            LoadCurrentScene();
        }
        else
        {
            Debug.LogWarning($"Invalid scene index: {index}");
        }
    }
    
    public void LoadSceneByName(string sceneName)
    {
        for (int i = 0; i < storyScenes.Length; i++)
        {
            if (storyScenes[i] == sceneName)
            {
                currentSceneIndex = i;
                LoadCurrentScene();
                return;
            }
        }
        
        Debug.LogWarning($"Scene not found in story sequence: {sceneName}");
    }
    
    private void LoadCurrentScene()
    {
        if (currentSceneIndex >= 0 && currentSceneIndex < storyScenes.Length)
        {
            string sceneName = storyScenes[currentSceneIndex];
            Debug.Log($"Loading story scene: {sceneName} (Index: {currentSceneIndex})");
            
            if (dialogueManager != null)
            {
                dialogueManager.LoadAndStartSceneFromResources($"StoryRoutes/{sceneName}");
            }
            else
            {
                Debug.LogError("DialogueManager not found!");
            }
        }
    }
    
    private void CompleteStory()
    {
        Debug.Log("Story sequence complete!");
        OnStoryComplete?.Invoke();
    }
    
    // Public getters
    public int CurrentSceneIndex => currentSceneIndex;
    public string CurrentSceneName => (currentSceneIndex >= 0 && currentSceneIndex < storyScenes.Length) 
        ? storyScenes[currentSceneIndex] : "Unknown";
    public bool IsStoryComplete => currentSceneIndex >= storyScenes.Length - 1;
    public float StoryProgress => (float)currentSceneIndex / (storyScenes.Length - 1);
    
    // Manual scene control for debugging
    [ContextMenu("Next Scene")]
    public void NextSceneDebug()
    {
        AdvanceToNextScene();
    }
    
    [ContextMenu("Previous Scene")]
    public void PreviousSceneDebug()
    {
        if (currentSceneIndex > 0)
        {
            currentSceneIndex--;
            LoadCurrentScene();
        }
    }
    
    [ContextMenu("Restart Story")]
    public void RestartStoryDebug()
    {
        StartStory();
    }
}
