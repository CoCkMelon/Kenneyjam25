using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;


public class OptionsController : MonoBehaviour
{
    /* ─────────── UI handles (all match the names in UXML) ─────────── */
    UIDocument     uiDocument;
    VisualElement  root;

    // Drop-downs
    DropdownField  screenSelect,
                   resolutionSelect,
                   windowModeSelect,
                   vsyncSelect,
                   aaSelect,
                   graphicsSelect,
                   virtjoySelect;

    // TextFields
    TextField      resolutionWidthField,
                   resolutionHeightField;

    // Toggles
    Toggle         camsmoothToggle,
                   developerModeToggle,
                   showFpsToggle;

    // Sliders
    Slider         soundVolumeSlider,
                   musicVolumeSlider;

    // Buttons
    Button         optionsResetButton,
                   saveButton,
                   backButton;

    // Labels
    Label          revertWarningLabel;


    /* ================================================================= */
    /*  Unity life-cycle                                                 */
    /* ================================================================= */
    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root       = uiDocument.rootVisualElement;

        CacheUIElements();
        RegisterCallbacks();
    }

    /* ================================================================= */
    /*  1.  Cache queries                                                */
    /* ================================================================= */
    void CacheUIElements()
    {
        screenSelect          = root.Q<DropdownField>("screen-select");
        resolutionSelect      = root.Q<DropdownField>("resolution-select");
        windowModeSelect      = root.Q<DropdownField>("window-mode-select");
        vsyncSelect           = root.Q<DropdownField>("vsync-select");
        aaSelect              = root.Q<DropdownField>("aa-select");
        graphicsSelect        = root.Q<DropdownField>("graphics-select");
        virtjoySelect         = root.Q<DropdownField>("virtjoy-select");

        resolutionWidthField  = root.Q<TextField>("resolution-width");
        resolutionHeightField = root.Q<TextField>("resolution-height");

        camsmoothToggle       = root.Q<Toggle>("camsmooth-toggle");
        developerModeToggle   = root.Q<Toggle>("developer-mode-toggle");
        showFpsToggle         = root.Q<Toggle>("show-fps-toggle");

        soundVolumeSlider     = root.Q<Slider>("sound-volume-slider");
        musicVolumeSlider     = root.Q<Slider>("music-volume-slider");

        optionsResetButton            = root.Q<Button>("options-reset-button");

        saveButton            = root.Q<Button>("save-button");
        backButton            = root.Q<Button>("back-button");

        revertWarningLabel    = root.Q<Label>("revert-warning");
    }

    /* ================================================================= */
    /*  2.  Register all callbacks                                       */
    /* ================================================================= */
    void RegisterCallbacks()
    {
        // Dropdowns
        screenSelect.     RegisterValueChangedCallback(OnScreenChanged);
        resolutionSelect. RegisterValueChangedCallback(OnResolutionChanged);
        windowModeSelect. RegisterValueChangedCallback(OnWindowModeChanged);
        vsyncSelect.      RegisterValueChangedCallback(OnVSyncChanged);
        aaSelect.         RegisterValueChangedCallback(OnAAModeChanged);
        graphicsSelect.   RegisterValueChangedCallback(OnGraphicsQualityChanged);
        virtjoySelect.    RegisterValueChangedCallback(OnVirtualJoystickChanged);

        // TextFields
        resolutionWidthField. RegisterValueChangedCallback(OnResolutionWidthEdited);
        resolutionHeightField.RegisterValueChangedCallback(OnResolutionHeightEdited);

        // Toggles
        camsmoothToggle.     RegisterValueChangedCallback(OnCameraSmoothingToggled);
        developerModeToggle. RegisterValueChangedCallback(OnDeveloperModeToggled);
        showFpsToggle.       RegisterValueChangedCallback(OnShowFpsToggled);

        // Sliders
        soundVolumeSlider. RegisterValueChangedCallback(OnSoundVolumeChanged);
        musicVolumeSlider. RegisterValueChangedCallback(OnMusicVolumeChanged);

        // Buttons
        saveButton.clicked += OnSaveClicked;
        backButton.clicked += OnBackClicked;
    }

    /* ================================================================= */
    /*  3.  Callback stubs (add your own logic inside)                   */
    /* ================================================================= */

    // --- Dropdowns ----------------------------------------------------
    void OnScreenChanged       (ChangeEvent<string> evt) {
        // Update resolutions

    }
    void OnResolutionChanged   (ChangeEvent<string> evt) { }
    void OnWindowModeChanged   (ChangeEvent<string> evt) { }
    void OnVSyncChanged        (ChangeEvent<string> evt) { }
    void OnAAModeChanged       (ChangeEvent<string> evt) { }
    void OnGraphicsQualityChanged(ChangeEvent<string> evt) { }
    void OnVirtualJoystickChanged(ChangeEvent<string> evt) { }

    // --- TextFields ---------------------------------------------------
    void OnResolutionWidthEdited (ChangeEvent<string> evt) { }
    void OnResolutionHeightEdited(ChangeEvent<string> evt) { }

    // --- Toggles ------------------------------------------------------
    void OnCameraSmoothingToggled(ChangeEvent<bool> evt) { }
    void OnDeveloperModeToggled  (ChangeEvent<bool> evt) { }
    void OnShowFpsToggled        (ChangeEvent<bool> evt) { }

    // --- Sliders ------------------------------------------------------
    void OnSoundVolumeChanged(ChangeEvent<float> evt) { }
    void OnMusicVolumeChanged(ChangeEvent<float> evt) { }

    // --- Buttons ------------------------------------------------------
    void OnSaveClicked() {
        // Apply appliable settings

        // Start timer

        // Save all settings
    }
    void OnBackClicked() {
        // If
    }

    /* ================================================================= */
    /*  4.  Utility functions                                            */
    /* ================================================================= */
    // private void StartRevertCountdown()
    // {
    //     if (revertSettingsCoroutine != null)
    //     {
    //         StopCoroutine(revertSettingsCoroutine);
    //     }
    //     revertSettingsCoroutine = StartCoroutine(RevertSettingsCountdown());
    // }
    //
    // private IEnumerator RevertSettingsCountdown()
    // {
    //     revertWarning.style.display = DisplayStyle.Flex;
    //     float timeLeft = 15f;
    //
    //     while (timeLeft > 0)
    //     {
    //         revertWarning.text = $"Settings will revert in {timeLeft:F0} seconds if not go back";
    //         yield return new WaitForSeconds(1f);
    //         timeLeft--;
    //     }
    //
    //     RevertSettings();
    // }
    //
    // private void RevertSettings()
    // {
    //     LoadCurrentSettings();
    //     ApplySettings();
    //     revertWarning.style.display = DisplayStyle.None;
    // }
    // private void ResetSettings()
    // {
    //     // Reset to default settings
    //     Screen.fullScreenMode = FullScreenMode.Windowed;
    //     Screen.SetResolution(640, 480, Screen.fullScreen);
    //     QualitySettings.antiAliasing = 0;
    //     QualitySettings.SetQualityLevel(0); // Lowest quality
    //
    //     LoadCurrentSettings(); // Reload UI to reflect changes
    //     Debug.Log("Settings reset to default");
    // }
}
