using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controls the ending scene with credits, final message, and return to main menu
/// </summary>
public class EndingSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI endingMessageText;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private ScrollRect creditsScrollRect;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Ending Messages")]
    [SerializeField] private string[] endingTitles = {
        "The End",
        "Mission Accomplished",
        "Journey's End",
        "Quantum Mastery Achieved"
    };
    
    [SerializeField] private string[] endingMessages = {
        "Through cunning and skill, you have mastered the art of sleight.\nThe quantum realm bends to your will, and reality itself\nacknowledges your supremacy.",
        "Your journey through the impossible has reached its conclusion.\nThe sleight now travels faster than light itself,\ntranscending the boundaries of space and time.",
        "The puzzles have been solved, the challenges overcome.\nYou have achieved what few thought possible -\ncontrol over the very fabric of existence.",
        "Thank you for playing this surreal adventure.\nMay your own journey be filled with\nwonder, mystery, and triumph."
    };
    
    [Header("Credits Content")]
    [TextArea(10, 20)]
    [SerializeField] private string creditsContent = @"KENNEYJAM 25
Surreal Sled Adventure

GAME DESIGN
- Puzzle Mechanics
- Vehicle Physics  
- Story Integration

PROGRAMMING
- Unity 3D Implementation
- C# Scripting (69+ files)
- Save System
- Scene Management

ART & DESIGN
- 3D Models & Materials
- Particle Effects
- UI Design
- Visual Effects

AUDIO
- Sound Effects
- Music Integration
- Audio Management

STORY & NARRATIVE
- YAML Dialogue System
- Character Development
- Branching Narratives

SPECIAL THANKS
- Kenney Game Jam Community
- Unity Technologies
- All Playtesters

CHARACTERS
- Dr. Mortimer Bones
- The Nutcracker
- Cornelius Frost
- The Sled Operator (You!)

TECHNOLOGIES USED
- Unity 2023.3+
- Universal Render Pipeline
- C# .NET Framework
- YAML Content System

Built with ❤️ for Kenney Jam 25

A surreal adventure where sleds defy physics
and puzzles reshape reality!

Thank you for playing!";
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem endingEffect;
    [SerializeField] private GameObject[] decorativeObjects;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color[] backgroundGradient;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip endingMusic;
    [SerializeField] private AudioClip buttonClickSound;
    
    [Header("Animation Settings")]
    [SerializeField] private float titleFadeInDuration = 2f;
    [SerializeField] private float messageFadeInDelay = 3f;
    [SerializeField] private float messageFadeInDuration = 2f;
    [SerializeField] private float creditsFadeInDelay = 6f;
    [SerializeField] private float creditsScrollSpeed = 30f;
    [SerializeField] private float buttonFadeInDelay = 8f;
    
    private SaveData playerSaveData;
    
    void Start()
    {
        InitializeEndingScene();
        StartCoroutine(PlayEndingSequence());
    }
    
    private void InitializeEndingScene()
    {
        // Load player data
        playerSaveData = SaveSystem.LoadGame();
        
        // Setup buttons
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Initially hide all UI elements
        SetUIElementsAlpha(0f);
        
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null && endingMusic != null)
        {
            audioSource.clip = endingMusic;
            audioSource.loop = true;
            audioSource.volume = 0.8f;
            audioSource.Play();
        }
        
        // Setup visual effects
        if (endingEffect != null)
            endingEffect.Play();
        
        foreach (GameObject obj in decorativeObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        
        // Setup credits content
        if (creditsText != null)
        {
            creditsText.text = creditsContent;
        }
        
        // Start background gradient animation
        if (backgroundGradient.Length > 0)
            StartCoroutine(AnimateBackgroundGradient());
    }
    
    private IEnumerator PlayEndingSequence()
    {
        // Fade in title
        yield return StartCoroutine(FadeInTitle());
        
        // Fade in ending message
        yield return StartCoroutine(FadeInEndingMessage());
        
        // Fade in credits and start scrolling
        yield return StartCoroutine(FadeInCredits());
        
        // Fade in buttons
        yield return StartCoroutine(FadeInButtons());
    }
    
    private IEnumerator FadeInTitle()
    {
        if (titleText == null) yield break;
        
        // Choose appropriate title
        string title = endingTitles[Random.Range(0, endingTitles.Length)];
        titleText.text = title;
        
        yield return StartCoroutine(FadeInTextElement(titleText, titleFadeInDuration));
        
        // Add emphasis effect
        yield return new WaitForSeconds(0.5f);
        titleText.fontSize *= 1.1f;
        yield return new WaitForSeconds(0.2f);
        titleText.fontSize /= 1.1f;
    }
    
    private IEnumerator FadeInEndingMessage()
    {
        yield return new WaitForSeconds(messageFadeInDelay);
        
        if (endingMessageText == null) yield break;
        
        // Choose message based on player performance
        string message = GetPersonalizedEndingMessage();
        endingMessageText.text = message;
        
        yield return StartCoroutine(FadeInTextElement(endingMessageText, messageFadeInDuration));
    }
    
    private IEnumerator FadeInCredits()
    {
        yield return new WaitForSeconds(creditsFadeInDelay);
        
        if (creditsText == null || creditsScrollRect == null) yield break;
        
        // Fade in credits
        yield return StartCoroutine(FadeInTextElement(creditsText, 2f));
        
        // Start scrolling credits
        StartCoroutine(ScrollCredits());
    }
    
    private IEnumerator ScrollCredits()
    {
        if (creditsScrollRect == null) yield break;
        
        // Start from bottom
        creditsScrollRect.verticalNormalizedPosition = 0f;
        
        while (creditsScrollRect.verticalNormalizedPosition < 1f)
        {
            creditsScrollRect.verticalNormalizedPosition += creditsScrollSpeed * Time.deltaTime / 1000f;
            yield return null;
        }
        
        // Keep at top
        creditsScrollRect.verticalNormalizedPosition = 1f;
    }
    
    private IEnumerator FadeInButtons()
    {
        yield return new WaitForSeconds(buttonFadeInDelay);
        
        Button[] buttons = { mainMenuButton, quitButton };
        
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
                
                yield return StartCoroutine(FadeInCanvasGroup(canvasGroup, 1f));
                button.interactable = true;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    
    private IEnumerator FadeInTextElement(TextMeshProUGUI textElement, float duration)
    {
        if (textElement == null) yield break;
        
        Color originalColor = textElement.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        textElement.color = transparentColor;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0f, originalColor.a, elapsedTime / duration);
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        textElement.color = originalColor;
    }
    
    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    private IEnumerator AnimateBackgroundGradient()
    {
        if (backgroundImage == null || backgroundGradient.Length == 0) yield break;
        
        int currentColorIndex = 0;
        
        while (true)
        {
            int nextColorIndex = (currentColorIndex + 1) % backgroundGradient.Length;
            Color startColor = backgroundGradient[currentColorIndex];
            Color endColor = backgroundGradient[nextColorIndex];
            
            float elapsedTime = 0f;
            float duration = 4f;
            
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                backgroundImage.color = Color.Lerp(startColor, endColor, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            currentColorIndex = nextColorIndex;
        }
    }
    
    private string GetPersonalizedEndingMessage()
    {
        if (playerSaveData != null)
        {
            // Choose message based on player's achievement
            if (playerSaveData.playtime < 300f) // Less than 5 minutes - speed run
                return endingMessages[1];
            else if (playerSaveData.timesRevived == 0) // No deaths - perfect run
                return endingMessages[2];
            else if (playerSaveData.puzzleProgress >= 3) // Completed all puzzles
                return endingMessages[0];
        }
        
        return endingMessages[3]; // Default thank you message
    }
    
    private void SetUIElementsAlpha(float alpha)
    {
        TextMeshProUGUI[] texts = { titleText, endingMessageText, creditsText };
        foreach (var text in texts)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }
        
        Button[] buttons = { mainMenuButton, quitButton };
        foreach (var button in buttons)
        {
            if (button != null)
            {
                CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
                
                canvasGroup.alpha = alpha;
                button.interactable = alpha >= 1f;
            }
        }
    }
    
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    private void GoToMainMenu()
    {
        PlayButtonSound();
        
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.GoToMainMenu();
        }
    }
    
    private void QuitGame()
    {
        PlayButtonSound();
        
        StartCoroutine(QuitAfterDelay());
    }
    
    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
