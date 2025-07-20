using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PuzzleHUDController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private UIDocument hudDocument;
    
    [Header("Settings")]
    [SerializeField] private bool showDebugPanel = false;
    [SerializeField] private bool showMobileControls = false;
    
    // Main UI Elements
    private Label puzzleTitle;
    private Label puzzleProgress;
    private Label currentPuzzleTime;
    private Label totalGameTime;
    private Label currentScore;
    private Label puzzleInstruction;
    private ProgressBar puzzleProgressBar;
    private Label progressText;
    private Button pauseButton;
    private Button hintButton;
    private Label hintsRemaining;
    
    // Completion UI
    private VisualElement completionOverlay;
    private Label completionTitle;
    private Label completionTime;
    private Label completionScore;
    private VisualElement completionStars;
    
    // Final completion UI
    private VisualElement gameCompleteOverlay;
    private Label gameCompleteTitle;
    private Label finalTime;
    private Label finalScore;
    private Label finalHints;
    private Button restartButton;
    private Button mainMenuButton;
    
    // Hint display
    private VisualElement hintDisplay;
    private Label hintText;
    
    // Loading overlay
    private VisualElement loadingOverlay;
    private Label loadingText;
    
    // Debug UI
    private VisualElement debugPanel;
    private Label fpsCounter;
    private Label debugInfo;
    
    // Mobile controls
    private VisualElement mobileControls;
    private Button mobileHint;
    private Button mobileReset;
    private Button mobilePause;
    
    void OnEnable()
    {
        InitializeUIElements();
        SetupEventHandlers();
        UpdateVisibility();
    }
    
    void Start()
    {
        // Subscribe to PuzzleGameManager events if available
        if (PuzzleGameManager.Instance != null)
        {
            PuzzleGameManager.Instance.OnScoreChanged += UpdateScore;
            PuzzleGameManager.Instance.OnHintUsed += OnHintUsed;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (PuzzleGameManager.Instance != null)
        {
            PuzzleGameManager.Instance.OnScoreChanged -= UpdateScore;
            PuzzleGameManager.Instance.OnHintUsed -= OnHintUsed;
        }
    }

    void Update()
    {
        UpdateFPSDisplay();
        UpdateMobileControlsVisibility();
    }
    
    private void InitializeUIElements()
    {
        var root = hudDocument.rootVisualElement;

        // Main UI elements
        puzzleTitle = root.Q<Label>("puzzle-title");
        puzzleProgress = root.Q<Label>("puzzle-progress");
        currentPuzzleTime = root.Q<Label>("current-puzzle-time");
        totalGameTime = root.Q<Label>("total-game-time");
        currentScore = root.Q<Label>("current-score");
        puzzleInstruction = root.Q<Label>("puzzle-instruction");
        puzzleProgressBar = root.Q<ProgressBar>("puzzle-progress-bar");
        progressText = root.Q<Label>("progress-text");
        hintsRemaining = root.Q<Label>("hints-remaining");
        
        // Buttons
        pauseButton = root.Q<Button>("pause-button");
        hintButton = root.Q<Button>("hint-button");
        
        // Completion UI
        completionOverlay = root.Q<VisualElement>("completion-overlay");
        completionTitle = root.Q<Label>("completion-title");
        completionTime = root.Q<Label>("completion-time");
        completionScore = root.Q<Label>("completion-score");
        completionStars = root.Q<VisualElement>("completion-stars");
        
        // Final completion UI
        gameCompleteOverlay = root.Q<VisualElement>("game-complete-overlay");
        gameCompleteTitle = root.Q<Label>("game-complete-title");
        finalTime = root.Q<Label>("final-time");
        finalScore = root.Q<Label>("final-score");
        finalHints = root.Q<Label>("final-hints");
        restartButton = root.Q<Button>("restart-button");
        mainMenuButton = root.Q<Button>("main-menu-button");
        
        // Hint display
        hintDisplay = root.Q<VisualElement>("hint-display");
        hintText = root.Q<Label>("hint-text");
        
        // Loading overlay
        loadingOverlay = root.Q<VisualElement>("loading-overlay");
        loadingText = root.Q<Label>("loading-text");
        
        // Debug UI
        debugPanel = root.Q<VisualElement>("debug-panel");
        fpsCounter = root.Q<Label>("fps-counter");
        debugInfo = root.Q<Label>("debug-info");
        
        // Mobile controls
        mobileControls = root.Q<VisualElement>("mobile-controls");
        mobileHint = root.Q<Button>("mobile-hint");
        mobileReset = root.Q<Button>("mobile-reset");
        mobilePause = root.Q<Button>("mobile-pause");
    }
    
    private void SetupEventHandlers()
    {
        // Main buttons
        if (pauseButton != null) 
            pauseButton.clicked += TogglePause;
        if (hintButton != null)
            hintButton.clicked += RequestHint;
        
        // Final completion buttons
        if (restartButton != null)
            restartButton.clicked += RestartGame;
        if (mainMenuButton != null)
            mainMenuButton.clicked += LoadMainMenu;
        
        // Mobile buttons
        if (mobileHint != null)
            mobileHint.clicked += RequestHint;
        if (mobileReset != null)
            mobileReset.clicked += ResetPuzzle;
        if (mobilePause != null)
            mobilePause.clicked += TogglePause;
    }
    
    private void UpdateVisibility()
    {
        // Debug panel visibility
        if (debugPanel != null)
            debugPanel.style.display = (showDebugPanel && Debug.isDebugBuild) ? DisplayStyle.Flex : DisplayStyle.None;
        
        // Mobile controls visibility will be updated in UpdateMobileControlsVisibility
    }
    
    private void UpdateMobileControlsVisibility()
    {
        if (mobileControls != null)
        {
            bool shouldShow = showMobileControls && Application.isMobilePlatform;
            mobileControls.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void UpdateFPSDisplay()
    {
        if (fpsCounter != null && Debug.isDebugBuild)
        {
            float fps = 1 / Time.unscaledDeltaTime;
            fpsCounter.text = $"FPS: {Mathf.RoundToInt(fps)}";
        }
    }
    
    // Public methods called by PuzzleGameManager
    public void OnPuzzleLoaded(int current, int total, string title)
    {
        if (puzzleTitle != null) 
            puzzleTitle.text = title;
        if (puzzleProgress != null)
            puzzleProgress.text = $"Puzzle {current} of {total}";
        
        // Hide completion overlay
        if (completionOverlay != null)
            completionOverlay.style.display = DisplayStyle.None;
    }
    
    public void UpdateGameTime(float totalTime)
    {
        if (totalGameTime != null)
            totalGameTime.text = $"Total: {FormatTime(totalTime)}";
    }
    
    public void UpdateCurrentPuzzleTime(float puzzleTime)
    {
        if (currentPuzzleTime != null)
            currentPuzzleTime.text = FormatTime(puzzleTime);
    }

    public void UpdateScore(int score)
    {
        if (currentScore != null)
            currentScore.text = $"Score: {score}";
    }
    
    public void UpdatePuzzleInstruction(string instruction)
    {
        if (puzzleInstruction != null)
            puzzleInstruction.text = instruction;
    }

    public void OnPuzzleCompleted(int puzzleIndex, float time, int score)
    {
        if (completionOverlay != null)
        {
            completionOverlay.style.display = DisplayStyle.Flex;
            StartCoroutine(HideCompletionOverlayAfterDelay(2f));
        }

        if (completionTitle != null)
            completionTitle.text = $"Puzzle {puzzleIndex} Complete!";
        if (completionTime != null)
            completionTime.text = $"Time: {FormatTime(time)}";
        if (completionScore != null)
            completionScore.text = $"+{score} Points!";
        
        // Animate stars
        AnimateStars();
    }
    
    public void OnAllPuzzlesCompleted(float totalTime, int finalScore)
    {
        if (gameCompleteOverlay != null)
            gameCompleteOverlay.style.display = DisplayStyle.Flex;
        
        if (gameCompleteTitle != null)
            gameCompleteTitle.text = "All Puzzles Complete!";
        if (finalTime != null)
            finalTime.text = $"Total Time: {FormatTime(totalTime)}";
        if (this.finalScore != null)
            this.finalScore.text = $"Final Score: {finalScore}";
        
        // Update hints used
        if (PuzzleGameManager.Instance != null && finalHints != null)
        {
            int hintsUsed = PuzzleGameManager.Instance.HintsUsed;
            int maxHints = PuzzleGameManager.Instance.MaxHintsAllowed * PuzzleGameManager.Instance.TotalPuzzles;
            finalHints.text = $"Hints Used: {hintsUsed}/{maxHints}";
        }
    }
    
    public void OnHintUsed(int hintsUsed)
    {
        if (PuzzleGameManager.Instance != null)
        {
            int maxHints = PuzzleGameManager.Instance.MaxHintsAllowed;
            if (hintsRemaining != null)
                hintsRemaining.text = $"{maxHints - hintsUsed}/{maxHints}";
            
            // Update hint button state
            if (hintButton != null)
                hintButton.SetEnabled(hintsUsed < maxHints);
        }
    }
    
    public void OnGamePausedChanged(bool isPaused)
    {
        // Update pause button appearance if needed
        if (pauseButton != null)
        {
            pauseButton.text = isPaused ? "▶" : "⏸";
        }
    }
    
    public void ShowHint(string hintMessage)
    {
        if (hintDisplay != null && hintText != null)
        {
            hintText.text = hintMessage;
            hintDisplay.style.display = DisplayStyle.Flex;
            StartCoroutine(HideHintAfterDelay(3f));
        }
    }
    
    public void ShowLoading(string message = "Loading Next Puzzle...")
    {
        if (loadingOverlay != null && loadingText != null)
        {
            loadingText.text = message;
            loadingOverlay.style.display = DisplayStyle.Flex;
        }
    }
    
    public void HideLoading()
    {
        if (loadingOverlay != null)
            loadingOverlay.style.display = DisplayStyle.None;
    }

    public void UpdatePuzzleProgress(float progress, int current, int total)
    {
        if (puzzleProgressBar != null)
            puzzleProgressBar.value = progress;
        if (progressText != null)
            progressText.text = $"{current}/{total}";
    }
    
    private void AnimateStars()
    {
        if (completionStars == null) return;
        
        var stars = completionStars.Children();
        StartCoroutine(AnimateStarsCoroutine(stars));
    }
    
    private IEnumerator AnimateStarsCoroutine(System.Collections.Generic.IEnumerable<VisualElement> stars)
    {
        int starIndex = 0;
        foreach (var star in stars)
        {
            yield return new WaitForSeconds(0.3f);
            // Simple scale animation simulation
            star.style.opacity = 1f;
            starIndex++;
        }
    }
    
    private IEnumerator HideCompletionOverlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (completionOverlay != null)
            completionOverlay.style.display = DisplayStyle.None;
    }
    
    private IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hintDisplay != null)
            hintDisplay.style.display = DisplayStyle.None;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }

    private void TogglePause()
    {
        if (PuzzleGameManager.Instance != null)
            PuzzleGameManager.Instance.TogglePause();
    }

    private void RequestHint()
    {
        if (PuzzleGameManager.Instance != null)
            PuzzleGameManager.Instance.RequestHint();
    }
    
    private void ResetPuzzle()
    {
        if (PuzzleGameManager.Instance != null)
            PuzzleGameManager.Instance.ResetCurrentPuzzle();
    }
    
    private void RestartGame()
    {
        if (PuzzleGameManager.Instance != null)
            PuzzleGameManager.Instance.RestartGame();
    }
    
    private void LoadMainMenu()
    {
        if (PuzzleGameManager.Instance != null)
            PuzzleGameManager.Instance.LoadMainMenu();
    }
    
    // Public methods for external control
    public void SetMobileControlsVisible(bool visible)
    {
        showMobileControls = visible;
        UpdateMobileControlsVisibility();
    }
    
    public void SetDebugPanelVisible(bool visible)
    {
        showDebugPanel = visible;
        UpdateVisibility();
    }
}
