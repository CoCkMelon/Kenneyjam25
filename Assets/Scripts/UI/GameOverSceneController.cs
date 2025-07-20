using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controls the Game Over Scene UI and functionality when the player fails
/// </summary>
public class GameOverSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Game Over Messages")]
    [SerializeField] private string[] gameOverMessages = {
        "Game Over",
        "Mission Failed",
        "Better luck next time...",
        "The sleight has failed..."
    };
    
    [SerializeField] private string[] failureReasons = {
        "Your sleight couldn't handle the quantum pressures",
        "The puzzles proved too challenging",
        "Time ran out before reaching light speed",
        "The journey ends here..."
    };
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip buttonClickSound;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject[] sadEffects;
    [SerializeField] private Color backgroundColor = Color.red;
    
    [Header("Animation Settings")]
    [SerializeField] private float textAnimationDuration = 2f;
    [SerializeField] private float buttonFadeInDelay = 4f;
    [SerializeField] private float buttonFadeInDuration = 1f;
    
    private SaveData playerSaveData;
    private Camera mainCamera;
    
    void Start()
    {
        InitializeGameOverScene();
        StartCoroutine(PlayGameOverSequence());
    }
    
    private void InitializeGameOverScene()
    {
        // Load player data
        playerSaveData = SaveSystem.LoadGame();
        
        // Setup camera background
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = backgroundColor;
        }
        
        // Setup buttons
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Initially hide buttons
        SetButtonsAlpha(0f);
        
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Play game over music
        if (audioSource != null && gameOverMusic != null)
        {
            audioSource.clip = gameOverMusic;
            audioSource.loop = true;
            audioSource.volume = 0.7f;
            audioSource.Play();
        }
        
        // Activate sad effects
        foreach (GameObject effect in sadEffects)
        {
            if (effect != null)
                effect.SetActive(true);
        }
    }
    
    private IEnumerator PlayGameOverSequence()
    {
        // Animate game over text
        yield return StartCoroutine(AnimateGameOverText());
        
        // Display failure reason
        yield return StartCoroutine(DisplayFailureReason());
        
        // Display survival stats
        yield return StartCoroutine(DisplaySurvivalStats());
        
        // Fade in buttons
        yield return StartCoroutine(FadeInButtons());
    }
    
    private IEnumerator AnimateGameOverText()
    {
        if (gameOverText == null) yield break;
        
        // Choose random game over message
        string message = gameOverMessages[Random.Range(0, gameOverMessages.Length)];
        gameOverText.text = "";
        
        // Type out the text character by character
        float characterDelay = textAnimationDuration / message.Length;
        
        for (int i = 0; i <= message.Length; i++)
        {
            gameOverText.text = message.Substring(0, i);
            yield return new WaitForSeconds(characterDelay);
        }
        
        // Add some dramatic effect
        yield return new WaitForSeconds(0.5f);
        
        // Flash the text
        for (int i = 0; i < 3; i++)
        {
            gameOverText.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            gameOverText.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private IEnumerator DisplayFailureReason()
    {
        if (reasonText == null) yield break;
        
        yield return new WaitForSeconds(0.5f);
        
        // Choose a failure reason
        string reason = failureReasons[Random.Range(0, failureReasons.Length)];
        reasonText.text = reason;
        reasonText.gameObject.SetActive(true);
        
        // Fade in the reason text
        Color originalColor = reasonText.color;
        reasonText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        
        float elapsedTime = 0f;
        float fadeInTime = 1f;
        
        while (elapsedTime < fadeInTime)
        {
            float alpha = Mathf.Lerp(0f, originalColor.a, elapsedTime / fadeInTime);
            reasonText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        reasonText.color = originalColor;
    }
    
    private IEnumerator DisplaySurvivalStats()
    {
        if (survivalTimeText == null) yield break;
        
        yield return new WaitForSeconds(0.5f);
        
        if (playerSaveData != null)
        {
            string timeString = FormatPlaytime(playerSaveData.playtime);
            survivalTimeText.text = $"Survival Time: {timeString}";
            survivalTimeText.gameObject.SetActive(true);
        }
    }
    
    private IEnumerator FadeInButtons()
    {
        yield return new WaitForSeconds(buttonFadeInDelay);
        
        float elapsedTime = 0f;
        while (elapsedTime < buttonFadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / buttonFadeInDuration);
            SetButtonsAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        SetButtonsAlpha(1f);
    }
    
    private void SetButtonsAlpha(float alpha)
    {
        Button[] buttons = { retryButton, mainMenuButton, quitButton };
        
        foreach (Button button in buttons)
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
    
    private string FormatPlaytime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        
        if (hours > 0)
            return $"{hours}h {minutes}m {seconds}s";
        else if (minutes > 0)
            return $"{minutes}m {seconds}s";
        else
            return $"{seconds}s";
    }
    
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    private void RetryGame()
    {
        PlayButtonSound();
        
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            // Restart from the beginning
            sceneManager.StartNewGame();
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
        
        // Wait a bit for sound to play
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
    
    // Called when player fails the game
    public static void TriggerGameOver()
    {
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.GoToGameOverScene();
        }
    }
    
    // Called when player health reaches zero
    public static void TriggerGameOverDeath()
    {
        TriggerGameOver();
    }
    
    // Called when player fails a critical puzzle
    public static void TriggerGameOverPuzzleFailed()
    {
        TriggerGameOver();
    }
    
    // Called when time runs out
    public static void TriggerGameOverTimeout()
    {
        TriggerGameOver();
    }
}
