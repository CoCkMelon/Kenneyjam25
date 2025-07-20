using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the FTL Cutscene scene with auto-starting dialogue
/// </summary>
public class FTLSceneController : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueScenePath = "04_faster_than_light";
    [SerializeField] private float autoStartDelay = 1f;
    
    [Header("Integration")]
    [SerializeField] private CutsceneManager cutsceneManager;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ambientMusic;
    
    private DialogueManager dialogueManager;
    private bool dialogueStarted = false;
    
    void Start()
    {
        InitializeFTLScene();
        StartCoroutine(AutoStartSequence());
    }
    
    private void InitializeFTLScene()
    {
        // Find dialogue manager
        dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }
        
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found! Please ensure DialogueManager is in the scene.");
            return;
        }
        
        // Find cutscene manager if not assigned
        if (cutsceneManager == null)
        {
            cutsceneManager = FindFirstObjectByType<CutsceneManager>();
        }
        
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null && ambientMusic != null)
        {
            audioSource.clip = ambientMusic;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
        
        Debug.Log("FTL Scene Controller initialized successfully");
    }
    
    private IEnumerator AutoStartSequence()
    {
        // Wait for initialization
        yield return new WaitForSeconds(autoStartDelay);
        
        // Start dialogue
        StartDialogue();
        
        // Start cutscene effects simultaneously if available
        if (cutsceneManager != null)
        {
            cutsceneManager.PlayLightSpeedCutscene();
        }
    }
    
    private void StartDialogue()
    {
        if (dialogueManager != null && !dialogueStarted)
        {
            Debug.Log($"Starting FTL dialogue scene: {dialogueScenePath}");
            dialogueManager.LoadAndStartSceneFromResources(dialogueScenePath);
            dialogueStarted = true;
        }
        else
        {
            Debug.LogWarning("Cannot start dialogue - DialogueManager is null or dialogue already started");
        }
    }
    
    // Called when dialogue finishes
    public void OnDialogueComplete()
    {
        Debug.Log("FTL dialogue complete, proceeding with scene transition");
        
        // Fade out audio
        if (audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutAudio());
        }
        
        // Trigger scene transition after a brief delay
        StartCoroutine(DelayedSceneTransition());
    }
    
    private IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        float fadeTime = 2f;
        
        while (elapsedTime < fadeTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        audioSource.Stop();
    }
    
    private IEnumerator DelayedSceneTransition()
    {
        yield return new WaitForSeconds(2f);
        
        // Trigger scene transition
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.GoToPuzzleScene();
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager not found!");
        }
    }
    
    void Update()
    {
        // Handle user input to skip or continue
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (dialogueManager != null)
            {
                dialogueManager.AdvanceDialogue();
            }
        }
        
        // Handle escape to skip entire cutscene
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }
    
    private void SkipCutscene()
    {
        Debug.Log("Skipping FTL cutscene");
        
        // Stop dialogue
        if (dialogueManager != null)
        {
            // Hide dialogue UI
            DialogueUIController uiController = DialogueUIController.Instance;
            if (uiController != null)
            {
                uiController.HideDialogue();
            }
        }
        
        // Stop cutscene
        if (cutsceneManager != null)
        {
            cutsceneManager.SkipCutscene();
        }
        
        // Immediate scene transition
        OnDialogueComplete();
    }
    
    // Public method for external triggers
    public void TriggerFTLSequence()
    {
        if (!dialogueStarted)
        {
            StartCoroutine(AutoStartSequence());
        }
    }
}
