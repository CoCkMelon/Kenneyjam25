using UnityEngine;
using System.Collections;

/// <summary>
/// Controls transition scenes with auto-starting dialogue
/// </summary>
public class TransitionDialogueController : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueScenePath = "05_puzzle_transition";
    [SerializeField] private float autoStartDelay = 1f;
    [SerializeField] private string nextSceneName = "PuzzleScene";
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject[] ambientEffects;
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.3f, 1f);
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip transitionMusic;
    [SerializeField] private AudioClip ambientSound;
    
    private DialogueManager dialogueManager;
    private Camera mainCamera;
    private bool dialogueStarted = false;
    private bool transitionComplete = false;
    
    void Start()
    {
        InitializeTransitionScene();
        StartCoroutine(AutoStartDialogue());
    }
    
    private void InitializeTransitionScene()
    {
        // Find components
        dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }
        
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = backgroundColor;
        }
        
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null)
        {
            if (transitionMusic != null)
            {
                audioSource.clip = transitionMusic;
                audioSource.loop = true;
                audioSource.volume = 0.6f;
                audioSource.Play();
            }
        }
        
        // Activate ambient effects
        foreach (GameObject effect in ambientEffects)
        {
            if (effect != null)
                effect.SetActive(true);
        }
        
        Debug.Log("Transition scene initialized");
    }
    
    private IEnumerator AutoStartDialogue()
    {
        yield return new WaitForSeconds(autoStartDelay);
        
        if (dialogueManager != null && !dialogueStarted)
        {
            Debug.Log($"Starting transition dialogue: {dialogueScenePath}");
            dialogueManager.LoadAndStartSceneFromResources(dialogueScenePath);
            dialogueStarted = true;
            
            // Start monitoring for dialogue completion
            StartCoroutine(MonitorDialogueCompletion());
        }
    }
    
    private IEnumerator MonitorDialogueCompletion()
    {
        // Wait for dialogue to start
        yield return new WaitForSeconds(0.5f);
        
        // Check if dialogue UI is active
        DialogueUIController uiController = DialogueUIController.Instance;
        
        while (!transitionComplete)
        {
            // Check if dialogue has finished
            bool dialogueActive = false;
            if (uiController != null)
            {
                // Check if dialogue UI is visible (this would need to be implemented in DialogueUIController)
                // For now, we'll use a simple timer approach
                yield return new WaitForSeconds(1f);
                
                // Check if dialogue manager has more lines
                DialogueLine currentLine = dialogueManager.GetCurrentLine();
                if (currentLine == null)
                {
                    // No current line means dialogue is finished
                    OnDialogueComplete();
                    break;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    private void OnDialogueComplete()
    {
        if (!transitionComplete)
        {
            Debug.Log("Transition dialogue complete");
            transitionComplete = true;
            StartCoroutine(CompleteTransition());
        }
    }
    
    private IEnumerator CompleteTransition()
    {
        // Fade out audio
        if (audioSource != null && audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutAudio());
        }
        
        // Brief pause
        yield return new WaitForSeconds(1f);
        
        // Transition to next scene
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            if (nextSceneName == "PuzzleScene")
                sceneManager.GoToPuzzleScene();
            else if (nextSceneName == "EndingScene")
                sceneManager.GoToEndingScene();
            else if (nextSceneName == "WinScene")
                sceneManager.GoToWinScene();
            else
            {
                Debug.LogWarning($"Unknown next scene: {nextSceneName}");
                sceneManager.GoToPuzzleScene(); // Default fallback
            }
        }
    }
    
    private IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        float fadeTime = 1.5f;
        
        while (elapsedTime < fadeTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        audioSource.Stop();
    }
    
    void Update()
    {
        if (!transitionComplete)
        {
            // Handle user input for dialogue progression
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (dialogueManager != null)
                {
                    dialogueManager.AdvanceDialogue();
                }
            }
            
            // Handle skip
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SkipTransition();
            }
        }
    }
    
    private void SkipTransition()
    {
        Debug.Log("Skipping transition");
        
        // Hide dialogue immediately
        DialogueUIController uiController = DialogueUIController.Instance;
        if (uiController != null)
        {
            uiController.HideDialogue();
        }
        
        // Complete transition
        if (!transitionComplete)
        {
            OnDialogueComplete();
        }
    }
    
    // Public method to set transition parameters
    public void SetTransitionParameters(string dialoguePath, string targetScene)
    {
        dialogueScenePath = dialoguePath;
        nextSceneName = targetScene;
    }
    
    // Public method to manually trigger dialogue start
    public void TriggerDialogue()
    {
        if (!dialogueStarted)
        {
            StartCoroutine(AutoStartDialogue());
        }
    }
}
