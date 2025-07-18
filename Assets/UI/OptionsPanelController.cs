using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq; // For LINQ extension methods (e.g., .Distinct(), .Select())
using System; // For Action

public class OptionsPanelController : MonoBehaviour
{
    // Assign your UXML file in the Inspector
    [SerializeField] private VisualTreeAsset m_UXML;

    // References to UI Elements
    private VisualElement optionsPanel;
    private Label optionTitle;
    private ScrollView optionScrollView;
    private VisualElement optionContainer;

    private DropdownField screenSelect;
    private DropdownField resolutionSelect;
    private TextField resolutionWidth;
    private TextField resolutionHeight;
    private DropdownField windowModeSelect;
    private DropdownField vsyncSelect;
    private DropdownField aaSelect;
    private DropdownField graphicsSelect;
    private Toggle camSmoothToggle;
    private DropdownField virtJoySelect;
    private Slider soundVolumeSlider;
    private Slider musicVolumeSlider;
    private Toggle developerModeToggle;
    private Toggle showFpsToggle;
    private Button saveButton;
    private Button backButton;
    private Label revertWarningLabel;

    private Coroutine revertTimerCoroutine;
    private const float REVERT_TIME = 15f; // Seconds
    private Action onPanelClosed; // Callback for when the panel is closed (e.g., Back button clicked)

    // Store settings temporarily for "Revert" functionality
    private FullScreenMode initialWindowMode;
    private Resolution initialResolution;
    private int initialVSyncCount;
    private int initialQualityLevel;
    private int initialAntiAliasing;
    private bool initialCameraSmoothing;
    private string initialVirtualJoystickMode;
    private float initialSoundVolume;
    private float initialMusicVolume;
    private bool initialDeveloperMode;
    private bool initialShowFPS;


    // Public method to open the panel with an optional callback
    public void OpenPanel(Action onCloseCallback = null)
    {
        onPanelClosed = onCloseCallback;
        // Ensure settings are loaded before opening
        GameSettings.LoadSettings();
        InitializeUI();
        StoreInitialSettings(); // Save current state for potential revert
        gameObject.SetActive(true); // Make sure the GameObject is active
    }

    // Public method to close the panel
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        onPanelClosed?.Invoke(); // Trigger callback if any
    }


    private void OnEnable()
    {
        // Get the UIDocument on this GameObject
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found on this GameObject.", this);
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        // Clear existing content to prevent duplicates if OnEnable is called multiple times
        root.Clear();

        // Instantiate the UXML and add it to the root
        optionsPanel = m_UXML.Instantiate();
        root.Add(optionsPanel);

        // Query and assign elements
        QueryElements(root);

        // Initialize UI with current settings
        InitializeUI();

        // Register callbacks for interaction
        RegisterCallbacks();

        // Store initial settings on panel load (or when OpenPanel is called)
        StoreInitialSettings();
    }

    private void OnDisable()
    {
        // Unregister callbacks to prevent memory leaks and unexpected behavior
        UnregisterCallbacks();
        // Stop any active coroutines
        if (revertTimerCoroutine != null)
        {
            StopCoroutine(revertTimerCoroutine);
            revertTimerCoroutine = null;
        }
        revertWarningLabel.style.display = DisplayStyle.None; // Hide warning when panel closes
    }

    private void QueryElements(VisualElement root)
    {
        optionTitle = root.Q<Label>("option-title");
        optionScrollView = root.Q<ScrollView>("option-scroll-view");
        optionContainer = root.Q<VisualElement>("option-container");

        screenSelect = root.Q<DropdownField>("screen-select");
        resolutionSelect = root.Q<DropdownField>("resolution-select");
        resolutionWidth = root.Q<TextField>("resolution-width");
        resolutionHeight = root.Q<TextField>("resolution-height");
        windowModeSelect = root.Q<DropdownField>("window-mode-select");
        vsyncSelect = root.Q<DropdownField>("vsync-select");
        aaSelect = root.Q<DropdownField>("aa-select");
        graphicsSelect = root.Q<DropdownField>("graphics-select");
        camSmoothToggle = root.Q<Toggle>("camsmooth-toggle");
        virtJoySelect = root.Q<DropdownField>("virtjoy-select");
        soundVolumeSlider = root.Q<Slider>("sound-volume-slider");
        musicVolumeSlider = root.Q<Slider>("music-volume-slider");
        developerModeToggle = root.Q<Toggle>("developer-mode-toggle");
        showFpsToggle = root.Q<Toggle>("show-fps-toggle");
        saveButton = root.Q<Button>("save-button");
        backButton = root.Q<Button>("back-button");
        revertWarningLabel = root.Q<Label>("revert-warning");

        // Set resolution text fields to read-only initially, they are just for display
        if (resolutionWidth != null) resolutionWidth.isReadOnly = true;
        if (resolutionHeight != null) resolutionHeight.isReadOnly = true;
    }

    private void InitializeUI()
    {
        // --- Screen Select (Display current screen's resolutions, not changing actual screen) ---
        if (screenSelect != null)
        {
            screenSelect.choices = new List<string> { "Primary Display" }; // Simplified
            screenSelect.value = "Primary Display"; // Always select "Primary Display"
            screenSelect.SetEnabled(false); // Disable, as it's not truly selectable across multiple monitors
        }

        // --- Resolution Dropdown ---
        if (resolutionSelect != null)
        {
            var resolutions = Screen.resolutions.Distinct().ToList(); // Get unique resolutions
            List<string> resolutionChoices = new List<string>();
            foreach (var res in resolutions)
            {
                resolutionChoices.Add($"{res.width}x{res.height}");
            }
            resolutionSelect.choices = resolutionChoices.Distinct().ToList(); // Ensure unique display strings

            // Set initial value
            string currentResolutionString = $"{GameSettings.CurrentResolution.width}x{GameSettings.CurrentResolution.height}";
            if (resolutionSelect.choices.Contains(currentResolutionString))
            {
                resolutionSelect.value = currentResolutionString;
            }
            else
            {
                resolutionSelect.value = resolutionSelect.choices.FirstOrDefault(); // Fallback
            }

            UpdateResolutionTextFields(GameSettings.CurrentResolution);
        }

        // --- Window Mode Dropdown ---
        if (windowModeSelect != null)
        {
            windowModeSelect.choices = Enum.GetNames(typeof(FullScreenMode)).ToList();
            windowModeSelect.value = GameSettings.WindowMode.ToString();
        }

        // --- VSync Dropdown ---
        if (vsyncSelect != null)
        {
            vsyncSelect.choices = new List<string> { "Off", "On (60 FPS)", "On (120 FPS)" };
            switch (GameSettings.VSyncCount)
            {
                case 0: vsyncSelect.value = "Off"; break;
                case 1: vsyncSelect.value = "On (60 FPS)"; break;
                case 2: vsyncSelect.value = "On (120 FPS)"; break;
                default: vsyncSelect.value = "Off"; break; // Default
            }
        }

        // --- Anti-Aliasing Dropdown ---
        if (aaSelect != null)
        {
            aaSelect.choices = new List<string> { "Off", "2x Multi Sampling", "4x Multi Sampling", "8x Multi Sampling" };
            string aaValue;
            switch (GameSettings.AntiAliasing)
            {
                case 0: aaValue = "Off"; break;
                case 2: aaValue = "2x Multi Sampling"; break;
                case 4: aaValue = "4x Multi Sampling"; break;
                case 8: aaValue = "8x Multi Sampling"; break;
                default: aaValue = "Off"; break; // Fallback
            }
            aaSelect.value = aaValue;
        }

        // --- Graphics Quality Dropdown ---
        if (graphicsSelect != null)
        {
            graphicsSelect.choices = QualitySettings.names.ToList();
            graphicsSelect.value = QualitySettings.names[GameSettings.QualityLevel];
        }

        // --- Camera Smoothing Toggle ---
        if (camSmoothToggle != null)
        {
            camSmoothToggle.value = GameSettings.CameraSmoothingEnabled;
        }

        // --- Onscreen Controls Dropdown ---
        if (virtJoySelect != null)
        {
            virtJoySelect.choices = new List<string> { "Off", "Thumbstick", "DPad" }; // Example options
            virtJoySelect.value = GameSettings.VirtualJoystickMode;
        }

        // --- Sound Volume Slider ---
        if (soundVolumeSlider != null)
        {
            // The UXML has lowValue="0" highValue="100", but AudioListener.volume is 0-1.
            // Convert between 0-100 range for UI and 0-1 for actual setting.
            soundVolumeSlider.value = GameSettings.SoundVolume * 100f;
        }

        // --- Music Volume Slider ---
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = GameSettings.MusicVolume * 100f;
        }

        // --- Developer Mode Toggle ---
        if (developerModeToggle != null)
        {
            developerModeToggle.value = GameSettings.DeveloperModeEnabled;
        }

        // --- Show FPS Toggle ---
        if (showFpsToggle != null)
        {
            showFpsToggle.value = GameSettings.ShowFPS;
        }

        // Hide warning initially
        if (revertWarningLabel != null)
        {
            revertWarningLabel.style.display = DisplayStyle.None;
        }
    }

    private void RegisterCallbacks()
    {
        if (screenSelect != null) screenSelect.RegisterValueChangedCallback(OnScreenChanged);
        if (resolutionSelect != null) resolutionSelect.RegisterValueChangedCallback(OnResolutionChanged);
        if (windowModeSelect != null) windowModeSelect.RegisterValueChangedCallback(OnWindowModeChanged);
        if (vsyncSelect != null) vsyncSelect.RegisterValueChangedCallback(OnVSyncChanged);
        if (aaSelect != null) aaSelect.RegisterValueChangedCallback(OnAAChanged);
        if (graphicsSelect != null) graphicsSelect.RegisterValueChangedCallback(OnGraphicsQualityChanged);
        if (camSmoothToggle != null) camSmoothToggle.RegisterValueChangedCallback(OnCamSmoothToggleChanged);
        if (virtJoySelect != null) virtJoySelect.RegisterValueChangedCallback(OnVirtJoyChanged);
        if (soundVolumeSlider != null) soundVolumeSlider.RegisterValueChangedCallback(OnSoundVolumeChanged);
        if (musicVolumeSlider != null) musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        if (developerModeToggle != null) developerModeToggle.RegisterValueChangedCallback(OnDeveloperModeToggleChanged);
        if (showFpsToggle != null) showFpsToggle.RegisterValueChangedCallback(OnShowFpsToggleChanged);

        if (saveButton != null) saveButton.clicked += OnSaveButtonClicked;
        if (backButton != null) backButton.clicked += OnBackButtonClicked;
    }

    private void UnregisterCallbacks()
    {
        if (screenSelect != null) screenSelect.UnregisterValueChangedCallback(OnScreenChanged);
        if (resolutionSelect != null) resolutionSelect.UnregisterValueChangedCallback(OnResolutionChanged);
        if (windowModeSelect != null) windowModeSelect.UnregisterValueChangedCallback(OnWindowModeChanged);
        if (vsyncSelect != null) vsyncSelect.UnregisterValueChangedCallback(OnVSyncChanged);
        if (aaSelect != null) aaSelect.UnregisterValueChangedCallback(OnAAChanged);
        if (graphicsSelect != null) graphicsSelect.UnregisterValueChangedCallback(OnGraphicsQualityChanged);
        if (camSmoothToggle != null) camSmoothToggle.UnregisterValueChangedCallback(OnCamSmoothToggleChanged);
        if (virtJoySelect != null) virtJoySelect.UnregisterValueChangedCallback(OnVirtJoyChanged);
        if (soundVolumeSlider != null) soundVolumeSlider.UnregisterValueChangedCallback(OnSoundVolumeChanged);
        if (musicVolumeSlider != null) musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);
        if (developerModeToggle != null) developerModeToggle.UnregisterValueChangedCallback(OnDeveloperModeToggleChanged);
        if (showFpsToggle != null) showFpsToggle.UnregisterValueChangedCallback(OnShowFpsToggleChanged);

        if (saveButton != null) saveButton.clicked -= OnSaveButtonClicked;
        if (backButton != null) backButton.clicked -= OnBackButtonClicked;
    }

    private void StoreInitialSettings()
    {
        initialWindowMode = GameSettings.WindowMode;
        initialResolution = GameSettings.CurrentResolution;
        initialVSyncCount = GameSettings.VSyncCount;
        initialQualityLevel = GameSettings.QualityLevel;
        initialAntiAliasing = GameSettings.AntiAliasing;
        initialCameraSmoothing = GameSettings.CameraSmoothingEnabled;
        initialVirtualJoystickMode = GameSettings.VirtualJoystickMode;
        initialSoundVolume = GameSettings.SoundVolume;
        initialMusicVolume = GameSettings.MusicVolume;
        initialDeveloperMode = GameSettings.DeveloperModeEnabled;
        initialShowFPS = GameSettings.ShowFPS;
    }

    private void RevertSettings()
    {
        GameSettings.SetWindowMode(initialWindowMode);
        GameSettings.SetResolution(initialResolution);
        GameSettings.SetVSyncCount(initialVSyncCount);
        GameSettings.SetQualityLevel(initialQualityLevel);
        GameSettings.SetAntiAliasing(initialAntiAliasing);
        GameSettings.SetCameraSmoothing(initialCameraSmoothing);
        GameSettings.SetVirtualJoystickMode(initialVirtualJoystickMode);
        GameSettings.SetSoundVolume(initialSoundVolume);
        GameSettings.SetMusicVolume(initialMusicVolume);
        GameSettings.SetDeveloperMode(initialDeveloperMode);
        GameSettings.SetShowFPS(initialShowFPS);
        GameSettings.ApplySettings(); // Apply the reverted settings
        InitializeUI(); // Refresh UI to reflect reverted settings
        Debug.Log("Settings reverted due to timeout or 'Back' button.");
    }

    // --- Event Handlers ---

    private void OnScreenChanged(ChangeEvent<string> evt)
    {
        // As discussed, this is simplified. For multiple physical displays,
        // you'd need more complex logic. For now, it's just a display.
        Debug.Log($"Screen changed to: {evt.newValue} (Functionality limited)");
    }

    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        if (string.IsNullOrEmpty(evt.newValue)) return;

        string[] dimensions = evt.newValue.Split('x');
        if (dimensions.Length == 2 && int.TryParse(dimensions[0], out int width) && int.TryParse(dimensions[1], out int height))
        {
            // Find the full Resolution object including refresh rate
            Resolution newRes = Screen.resolutions.FirstOrDefault(r => r.width == width && r.height == height);

            if (newRes.width != 0) // Check if a valid resolution was found
            {
                GameSettings.SetResolution(newRes);
                UpdateResolutionTextFields(newRes);
                StartRevertTimer();
            }
            else
            {
                Debug.LogWarning($"Could not find a matching resolution for {evt.newValue}.");
            }
        }
    }

    private void UpdateResolutionTextFields(Resolution res)
    {
        if (resolutionWidth != null) resolutionWidth.value = res.width.ToString();
        if (resolutionHeight != null) resolutionHeight.value = res.height.ToString();
    }

    private void OnWindowModeChanged(ChangeEvent<string> evt)
    {
        if (Enum.TryParse(evt.newValue, out FullScreenMode mode))
        {
            GameSettings.SetWindowMode(mode);
            StartRevertTimer();
        }
    }

    private void OnVSyncChanged(ChangeEvent<string> evt)
    {
        int vsync = 0;
        switch (evt.newValue)
        {
            case "Off": vsync = 0; break;
            case "On (60 FPS)": vsync = 1; break;
            case "On (120 FPS)": vsync = 2; break;
        }
        GameSettings.SetVSyncCount(vsync);
        StartRevertTimer();
    }

    private void OnAAChanged(ChangeEvent<string> evt)
    {
        int aa = 0;
        switch (evt.newValue)
        {
            case "Off": aa = 0; break;
            case "2x Multi Sampling": aa = 2; break;
            case "4x Multi Sampling": aa = 4; break;
            case "8x Multi Sampling": aa = 8; break;
        }
        GameSettings.SetAntiAliasing(aa);
        StartRevertTimer();
    }

    private void OnGraphicsQualityChanged(ChangeEvent<string> evt)
    {
        int qualityIndex = Array.IndexOf(QualitySettings.names, evt.newValue);
        if (qualityIndex != -1)
        {
            GameSettings.SetQualityLevel(qualityIndex);
            StartRevertTimer();
        }
    }

    private void OnCamSmoothToggleChanged(ChangeEvent<bool> evt)
    {
        GameSettings.SetCameraSmoothing(evt.newValue);
        StartRevertTimer();
    }

    private void OnVirtJoyChanged(ChangeEvent<string> evt)
    {
        GameSettings.SetVirtualJoystickMode(evt.newValue);
        StartRevertTimer();
    }

    private void OnSoundVolumeChanged(ChangeEvent<float> evt)
    {
        // Convert from 0-100 UI range to 0-1 game range
        GameSettings.SetSoundVolume(evt.newValue / 100f);
        StartRevertTimer();
    }

    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        // Convert from 0-100 UI range to 0-1 game range
        GameSettings.SetMusicVolume(evt.newValue / 100f);
        StartRevertTimer();
    }

    private void OnDeveloperModeToggleChanged(ChangeEvent<bool> evt)
    {
        GameSettings.SetDeveloperMode(evt.newValue);
        StartRevertTimer();
    }

    private void OnShowFpsToggleChanged(ChangeEvent<bool> evt)
    {
        GameSettings.SetShowFPS(evt.newValue);
        StartRevertTimer();
    }

    private void OnSaveButtonClicked()
    {
        // Apply all current settings to Unity and save to PlayerPrefs
        GameSettings.ApplySettings();
        GameSettings.SaveSettings();

        // Stop the revert timer and hide warning
        if (revertTimerCoroutine != null)
        {
            StopCoroutine(revertTimerCoroutine);
            revertTimerCoroutine = null;
        }
        revertWarningLabel.style.display = DisplayStyle.None;

        Debug.Log("Settings saved and applied.");
        ClosePanel(); // Close the panel after saving
    }

    private void OnBackButtonClicked()
    {
        // Revert settings to what they were when the panel opened
        RevertSettings();
        ClosePanel(); // Close the panel
    }

    // --- Revert Timer Logic ---
    private void StartRevertTimer()
    {
        // Stop any existing timer
        if (revertTimerCoroutine != null)
        {
            StopCoroutine(revertTimerCoroutine);
        }

        // Apply the settings immediately to see changes
        GameSettings.ApplySettings();

        // Show the warning label
        revertWarningLabel.style.display = DisplayStyle.Flex;

        // Start new timer
        revertTimerCoroutine = StartCoroutine(RevertTimer());
    }

    private System.Collections.IEnumerator RevertTimer()
    {
        float timer = REVERT_TIME;
        while (timer > 0)
        {
            // You could update the label text here to show remaining time
            revertWarningLabel.text = $"Settings will revert in {Mathf.CeilToInt(timer)} seconds if not confirmed";
            timer -= Time.deltaTime;
            yield return null;
        }

        // Timer finished, revert settings
        RevertSettings();
        revertWarningLabel.style.display = DisplayStyle.None;
        revertWarningLabel.text = "Settings will revert in 15 seconds if not confirmed"; // Reset text
        revertTimerCoroutine = null; // Clear coroutine reference
    }
}
