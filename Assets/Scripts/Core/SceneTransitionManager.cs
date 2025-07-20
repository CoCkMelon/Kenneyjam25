using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Manages all scene transitions and game flow between different game scenes
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string sleighSpeedScene = "SleighSpeed";
    [SerializeField] private string puzzleScene = "PuzzleScene";
    [SerializeField] private string ftlCutsceneScene = "FTLCutscene";
    [SerializeField] private string transitionScene = "TransitionScene";
    [SerializeField] private string winScene = "WinScene";
    [SerializeField] private string gameOverScene = "GameOverScene";
    [SerializeField] private string endingScene = "EndingScene";

    [Header("Transition Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private GameObject fadeCanvas;
    
    [Header("Events")]
    public UnityEvent OnSceneTransitionStart;
    public UnityEvent OnSceneTransitionComplete;

    // Singleton pattern
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SceneTransitionManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneTransitionManager");
                    instance = go.AddComponent<SceneTransitionManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Setup fade canvas if not assigned
        if (fadeCanvas == null)
        {
            SetupFadeCanvas();
        }
    }

    private void SetupFadeCanvas()
    {
        // Create a simple fade overlay canvas
        GameObject canvas = new GameObject("FadeCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComponent.sortingOrder = 100;
        
        UnityEngine.UI.CanvasScaler scaler = canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(canvas.transform);
        
        UnityEngine.UI.Image image = fadePanel.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.black;
        
        RectTransform rectTransform = fadePanel.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        fadeCanvas = canvas;
        DontDestroyOnLoad(fadeCanvas);
        
        // Start with fade canvas invisible
        SetFadeAlpha(0f);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeCanvas != null)
        {
            UnityEngine.UI.Image fadeImage = fadeCanvas.GetComponentInChildren<UnityEngine.UI.Image>();
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = alpha;
                fadeImage.color = color;
            }
        }
    }

    // Public scene transition methods
    public void GoToMainMenu()
    {
        StartCoroutine(TransitionToScene(mainMenuScene));
    }

    public void StartNewGame()
    {
        StartCoroutine(TransitionToScene(sleighSpeedScene));
    }

    public void GoToPuzzleScene()
    {
        StartCoroutine(TransitionToScene(puzzleScene));
    }

    public void GoToFTLCutscene()
    {
        StartCoroutine(TransitionToScene(ftlCutsceneScene));
    }

    public void GoToTransition()
    {
        StartCoroutine(TransitionToScene(transitionScene));
    }

    public void GoToWinScene()
    {
        StartCoroutine(TransitionToScene(winScene));
    }

    public void GoToGameOverScene()
    {
        StartCoroutine(TransitionToScene(gameOverScene));
    }

    public void GoToEndingScene()
    {
        StartCoroutine(TransitionToScene(endingScene));
    }

    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(TransitionToScene(currentScene));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        OnSceneTransitionStart?.Invoke();
        
        // Fade out
        yield return StartCoroutine(FadeOut());
        
        // Load new scene
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
        {
            yield return null;
        }
        
        // Wait one frame for scene to fully initialize
        yield return null;
        
        // Fade in
        yield return StartCoroutine(FadeIn());
        
        OnSceneTransitionComplete?.Invoke();
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
            SetFadeAlpha(alpha);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        SetFadeAlpha(1f);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            SetFadeAlpha(alpha);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        SetFadeAlpha(0f);
    }

    // Utility methods for checking current scene
    public bool IsInScene(string sceneName)
    {
        return SceneManager.GetActiveScene().name == sceneName;
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // Game flow control methods
    public void OnLightSpeedReached()
    {
        Debug.Log("Light speed reached! Transitioning to FTL Cutscene...");
        GoToFTLCutscene();
    }

    public void OnPuzzleCompleted()
    {
        Debug.Log("Puzzle completed! Transitioning to next scene...");
        GoToTransition();
    }

    public void OnGameWin()
    {
        Debug.Log("Player won! Going to win scene...");
        GoToWinScene();
    }

    public void OnGameOver()
    {
        Debug.Log("Game over! Going to game over scene...");
        GoToGameOverScene();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
