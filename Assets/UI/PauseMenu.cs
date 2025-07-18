// PauseMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [Header("Scene to load when <Back to main menu> is pressed")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    // Reference will be auto-filled if the script lives on the same
    // GameObject as the UIDocument
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private OptionsMenuController optionsMenuController;
    [SerializeField] private GameHUDController gameHUDController;

    // Cached visual elements
    private Button resumeBtn;
    private Button optionsBtn;
    private Button toMenuBtn;
    private Button quitBtn;

    private VisualElement root;

    // ──────────────────────────────────────────────────────────────
    // Initialisation
    // ──────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
        optionsMenuController.ApplySettings = Show;
    }

    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;

        // 1) Grab buttons by their UXML name attribute
        resumeBtn = root.Q<Button>("resume");
        optionsBtn = root.Q<Button>("options");
        toMenuBtn = root.Q<Button>("to-menu");
        quitBtn   = root.Q<Button>("quit");

        // 2) Hook up callbacks
        resumeBtn?.RegisterCallback<ClickEvent>(OnResumeClicked);
        optionsBtn?.RegisterCallback<ClickEvent>(OnOptionsClicked);
        toMenuBtn?.RegisterCallback<ClickEvent>(OnToMenuClicked);
        quitBtn?.RegisterCallback<ClickEvent>(OnQuitClicked);
    }

    private void OnDisable()
    {
        // Always unhook to avoid memory leaks
        resumeBtn?.UnregisterCallback<ClickEvent>(OnResumeClicked);
        optionsBtn?.UnregisterCallback<ClickEvent>(OnOptionsClicked);
        toMenuBtn?.UnregisterCallback<ClickEvent>(OnToMenuClicked);
        quitBtn?.UnregisterCallback<ClickEvent>(OnQuitClicked);
    }

    // ──────────────────────────────────────────────────────────────
    // Public helpers (so other scripts can show/hide the menu)
    // ──────────────────────────────────────────────────────────────
    public void Show()
    {
        gameObject.SetActive(true);
        optionsMenu.SetActive(false);
        root.style.display  = DisplayStyle.Flex;
        Time.timeScale      = 0f;   // Pause gameplay
    }

    public void Hide()
    {
        Time.timeScale      = 1f;   // Resume gameplay
        root.style.display  = DisplayStyle.None;
        gameObject.SetActive(false);
    }

    // ──────────────────────────────────────────────────────────────
    // Button handlers
    // ──────────────────────────────────────────────────────────────
    private void OnResumeClicked(ClickEvent _)
    {
        Hide();
        gameHUDController.ResumeGame();
    }

    private void OnOptionsClicked(ClickEvent _)
    {
        optionsMenu.SetActive(true);
        root.style.display  = DisplayStyle.None;
        gameObject.SetActive(false);
    }

    private void OnToMenuClicked(ClickEvent _)
    {
        Hide();           // make sure timescale is reset
        SceneManager.LoadScene(mainMenuScene);
    }

    private void OnQuitClicked(ClickEvent _)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
