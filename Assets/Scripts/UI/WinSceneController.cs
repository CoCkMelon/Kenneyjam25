using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controls the Win Scene UI and functionality when the player successfully completes the game
/// </summary>
public class WinSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI congratulationsText;
    [SerializeField] private TextMeshProUGUI completionTimeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    [Header("Victory Settings")]
    [SerializeField] private string[] congratulationMessages = {
        "Congratulations!",
        "You've mastered the art of sleight!",
        "Light speed achieved!",
        "Victory through cunning and skill!"
    };
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip buttonClickSound;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem celebrationEffect;
    [SerializeField] private GameObject[] celebrationObjects;
    
    [Header("Animation Settings")]
    [SerializeField] private float textAnimationDuration = 2f;
    [SerializeField] private float buttonFadeInDelay = 3f;
    [SerializeField] private float buttonFadeInDuration = 1f;
    
    private SaveData playerSaveData;
    
    void Start()
    {
        InitializeWinScene();
        StartCoroutine(PlayWinSequence());
    }
    
    private void InitializeWinScene()
    {
        // Load player data
        playerSaveData = SaveSystem.LoadGame();
        
        // Setup buttons
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Initially hide buttons
        SetButtonsAlpha(0f);
        
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Play victory music
        if (audioSource != null && victoryMusic != null)
        {
            audioSource.clip = victoryMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Start celebration effects
        if (celebrationEffect != null)
            celebrationEffect.Play();
        
        foreach (GameObject celebrationObject in celebrationObjects)
        {
            if (celebrationObject != null)
                celebrationObject.SetActive(true);
        }
    }
    
    private IEnumerator PlayWinSequence()
    {
        // Animate congratulations text
        yield return StartCoroutine(AnimateCongratsText());
        
        // Display completion stats
        yield return StartCoroutine(DisplayCompletionStats());
        
        // Fade in buttons
        yield return StartCoroutine(FadeInButtons());
    }
    
    private IEnumerator AnimateCongratsText()
    {
        if (congratulationsText == null) yield break;
        
        // Choose random congratulations message
        string message = congratulationMessages[Random.Range(0, congratulationMessages.Length)];
        congratulationsText.text = "";
        
        // Type out the text character by character
        float characterDelay = textAnimationDuration / message.Length;
        
        for (int i = 0; i <= message.Length; i++)
        {
            congratulationsText.text = message.Substring(0, i);
            yield return new WaitForSeconds(characterDelay);
        }
        
        // Add some emphasis
        yield return new WaitForSeconds(0.5f);
        congratulationsText.fontSize *= 1.2f;
        yield return new WaitForSeconds(0.2f);
        congratulationsText.fontSize /= 1.2f;
    }
    
    private IEnumerator DisplayCompletionStats()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Display completion time
        if (completionTimeText != null && playerSaveData != null)
        {
            string timeString = FormatPlaytime(playerSaveData.playtime);
            completionTimeText.text = $"Completion Time: {timeString}";
            completionTimeText.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(0.3f);
        
        // Display score (based on various factors)
        if (scoreText != null && playerSaveData != null)
        {
            int score = CalculateScore();
            scoreText.text = $"Final Score: {score:N0}";
            scoreText.gameObject.SetActive(true);
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
        Button[] buttons = { playAgainButton, mainMenuButton, quitButton };
        
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
    
    private int CalculateScore()
    {
        if (playerSaveData == null) return 0;
        
        int baseScore = 10000;
        
        // Time bonus (faster completion = higher score)
        float timeBonusMultiplier = Mathf.Max(0.1f, 2f - (playerSaveData.playtime / 600f)); // 10 minutes baseline
        int timeBonus = Mathf.RoundToInt(baseScore * timeBonusMultiplier);
        
        // Health bonus
        int healthBonus = Mathf.RoundToInt(playerSaveData.playerHealth * 10);
        
        // Currency bonus
        int currencyBonus = playerSaveData.currency * 5;
        
        // Survival bonus (fewer deaths = higher score)
        int survivalBonus = Mathf.Max(0, 5000 - (playerSaveData.timesRevived * 500));
        
        return baseScore + timeBonus + healthBonus + currencyBonus + survivalBonus;
    }
    
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    private void PlayAgain()
    {
        PlayButtonSound();
        
        // Delete current save and start new game
        SaveSystem.DeleteSaveFile();
        
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
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
    
    // Called when player wins the game
    public static void TriggerWin()
    {
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.GoToWinScene();
        }
    }
}
