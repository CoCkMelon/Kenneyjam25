using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controls transition scenes with narrative text and automatic progression
/// </summary>
public class TransitionSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI narrativeText;
    [SerializeField] private TextMeshProUGUI continuePrompt;
    [SerializeField] private Button skipButton;
    [SerializeField] private Image backgroundImage;
    
    [Header("Transition Settings")]
    [SerializeField] private string[] transitionMessages = {
        "The sleight has reached incredible speeds...",
        "Reality bends around you as you approach the impossible...",
        "The quantum realm beckons with new challenges...",
        "Your mastery grows with each challenge overcome...",
        "The final test awaits..."
    };
    
    [SerializeField] private string nextSceneName = "EndingScene";
    [SerializeField] private float textDisplayDuration = 3f;
    [SerializeField] private float autoProgressDelay = 5f;
    [SerializeField] private bool allowSkip = true;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem ambientEffect;
    [SerializeField] private Color[] backgroundColors;
    [SerializeField] private float colorTransitionSpeed = 1f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip narrativeMusic;
    [SerializeField] private AudioClip transitionSound;
    
    private bool isTransitioning = false;
    private int currentMessageIndex = 0;
    
    void Start()
    {
        InitializeTransition();
        StartCoroutine(PlayTransitionSequence());
    }
    
    private void InitializeTransition()
    {
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null && narrativeMusic != null)
        {
            audioSource.clip = narrativeMusic;
            audioSource.loop = true;
            audioSource.volume = 0.6f;
            audioSource.Play();
        }
        
        // Setup skip button
        if (skipButton != null && allowSkip)
        {
            skipButton.onClick.AddListener(SkipTransition);
            skipButton.gameObject.SetActive(true);
        }
        else if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
        }
        
        // Start ambient effects
        if (ambientEffect != null)
            ambientEffect.Play();
        
        // Initially hide continue prompt
        if (continuePrompt != null)
            continuePrompt.gameObject.SetActive(false);
        
        // Initialize text
        if (narrativeText != null)
            narrativeText.text = "";
        
        // Start background color cycling if available
        if (backgroundColors.Length > 0)
            StartCoroutine(CycleBackgroundColors());
    }
    
    private IEnumerator PlayTransitionSequence()
    {
        // Choose appropriate message based on game state or random
        string message = GetTransitionMessage();
        
        // Type out the narrative text
        yield return StartCoroutine(TypeOutText(message));
        
        // Show continue prompt or auto-progress
        if (allowSkip)
        {
            ShowContinuePrompt();
            yield return StartCoroutine(WaitForInput());
        }
        else
        {
            yield return new WaitForSeconds(autoProgressDelay);
        }
        
        // Transition to next scene
        yield return StartCoroutine(TransitionToNextScene());
    }
    
    private string GetTransitionMessage()
    {
        // Could be based on save data, current progress, etc.
        SaveData saveData = SaveSystem.LoadGame();
        
        if (saveData != null)
        {
            // Choose message based on player progress or achievements
            if (saveData.puzzleProgress >= 3)
                return transitionMessages[4]; // Final test
            else if (saveData.puzzleProgress >= 2)
                return transitionMessages[3]; // Mastery grows
            else if (saveData.puzzleProgress >= 1)
                return transitionMessages[2]; // Quantum realm
            else
                return transitionMessages[1]; // Reality bends
        }
        
        return transitionMessages[0]; // Default message
    }
    
    private IEnumerator TypeOutText(string text)
    {
        if (narrativeText == null) yield break;
        
        narrativeText.text = "";
        float characterDelay = textDisplayDuration / text.Length;
        
        for (int i = 0; i <= text.Length; i++)
        {
            narrativeText.text = text.Substring(0, i);
            
            // Play subtle typing sound
            if (audioSource != null && transitionSound != null && i < text.Length)
            {
                audioSource.PlayOneShot(transitionSound, 0.1f);
            }
            
            yield return new WaitForSeconds(characterDelay);
        }
    }
    
    private void ShowContinuePrompt()
    {
        if (continuePrompt != null)
        {
            continuePrompt.gameObject.SetActive(true);
            StartCoroutine(PulseContinuePrompt());
        }
    }
    
    private IEnumerator PulseContinuePrompt()
    {
        while (continuePrompt.gameObject.activeInHierarchy)
        {
            float alpha = Mathf.PingPong(Time.time, 1f);
            Color color = continuePrompt.color;
            color.a = alpha;
            continuePrompt.color = color;
            yield return null;
        }
    }
    
    private IEnumerator WaitForInput()
    {
        bool continuePressed = false;
        
        while (!continuePressed && !isTransitioning)
        {
            if (Input.GetKeyDown(KeyCode.Space) || 
                Input.GetKeyDown(KeyCode.Return) || 
                Input.GetKeyDown(KeyCode.Escape) ||
                Input.GetMouseButtonDown(0))
            {
                continuePressed = true;
            }
            
            yield return null;
        }
        
        if (continuePrompt != null)
            continuePrompt.gameObject.SetActive(false);
    }
    
    private IEnumerator CycleBackgroundColors()
    {
        if (backgroundImage == null || backgroundColors.Length == 0) yield break;
        
        int currentColorIndex = 0;
        
        while (!isTransitioning)
        {
            int nextColorIndex = (currentColorIndex + 1) % backgroundColors.Length;
            Color startColor = backgroundColors[currentColorIndex];
            Color endColor = backgroundColors[nextColorIndex];
            
            float elapsedTime = 0f;
            float duration = 3f; // Time to transition between colors
            
            while (elapsedTime < duration && !isTransitioning)
            {
                float t = elapsedTime / duration;
                backgroundImage.color = Color.Lerp(startColor, endColor, t);
                elapsedTime += Time.deltaTime * colorTransitionSpeed;
                yield return null;
            }
            
            currentColorIndex = nextColorIndex;
        }
    }
    
    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;
        
        // Fade out audio
        if (audioSource != null && audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            float elapsedTime = 0f;
            float fadeTime = 1f;
            
            while (elapsedTime < fadeTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            audioSource.Stop();
        }
        
        // Stop effects
        if (ambientEffect != null)
            ambientEffect.Stop();
        
        // Transition to next scene
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            if (nextSceneName == "EndingScene")
                sceneManager.GoToEndingScene();
            else if (nextSceneName == "WinScene")
                sceneManager.GoToWinScene();
            else if (nextSceneName == "PuzzleScene")
                sceneManager.GoToPuzzleScene();
            else
            {
                // Generic scene transition
                Debug.Log($"Transitioning to {nextSceneName}");
                // sceneManager.TransitionToScene(nextSceneName); // If we add this method
            }
        }
    }
    
    private void SkipTransition()
    {
        if (!isTransitioning)
        {
            StopAllCoroutines();
            StartCoroutine(TransitionToNextScene());
        }
    }
    
    // Public method to set transition parameters from other scripts
    public void SetTransitionParameters(string message, string targetScene, float displayTime = 3f)
    {
        transitionMessages[0] = message;
        nextSceneName = targetScene;
        textDisplayDuration = displayTime;
    }
}
