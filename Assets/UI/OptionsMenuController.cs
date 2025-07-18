using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Toolkit")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Risky-change revert")]
    [Tooltip("Time (seconds) before an un-confirmed resolution / window-mode change is rolled back.")]
    [SerializeField] private float revertTimeout = 15f;

    // Function that other script set to apply settings
    public Action ApplySettings;

    /* ───── cached UI handles ───── */
    DropdownField displayDrop, resolutionDrop, windowModeDrop,
                  vSyncDrop, aaDrop, qualityDrop, vJoyDrop;
    Toggle camSmoothT, devModeT, showFpsT;
    Slider sVolumeS, mVolumeS;
    Button optResetBtn, saveBtn, backBtn;
    Label  revertLbl;

    /* ───── local state ───── */
    SettingsData beforeChange;   // snapshot when menu opened / last confirmed
    SettingsData pending;        // what the user is about to commit
    Coroutine    revertCo;

    /* ============================================================ */
    #region Unity life-cycle
    /* ============================================================ */

    void OnEnable()
    {
        if (!uiDocument) uiDocument = GetComponent<UIDocument>();
        CacheControls();
        BuildStaticChoiceLists();
        ReadStoredSettingsIntoUI();
        RegisterCallbacks();
    }
    #endregion


    /* ============================================================ */
    #region Setup helpers
    /* ============================================================ */

    void CacheControls()
    {
        var root = uiDocument.rootVisualElement;

        displayDrop    = root.Q<DropdownField>("screen-select");
        resolutionDrop = root.Q<DropdownField>("resolution-select");
        windowModeDrop = root.Q<DropdownField>("window-mode-select");
        vSyncDrop      = root.Q<DropdownField>("vsync-select");
        aaDrop         = root.Q<DropdownField>("aa-select");
        qualityDrop    = root.Q<DropdownField>("graphics-select");
        vJoyDrop       = root.Q<DropdownField>("virtjoy-select");

        camSmoothT     = root.Q<Toggle>("camsmooth-toggle");
        devModeT       = root.Q<Toggle>("developer-mode-toggle");
        showFpsT       = root.Q<Toggle>("show-fps-toggle");

        sVolumeS       = root.Q<Slider>("sound-volume-slider");
        mVolumeS       = root.Q<Slider>("music-volume-slider");

        saveBtn        = root.Q<Button>("save-button");
        optResetBtn    = root.Q<Button>("options-reset-button");
        backBtn        = root.Q<Button>("back-button");
        revertLbl      = root.Q<Label>("revert-warning");
    }

    void BuildStaticChoiceLists()
    {
        /* Displays (single monitor for simplicity)                  */
        displayDrop.choices = new List<string> { "Display 1" };

        /* Resolutions (largest first)                               */
        var resChoices = new List<string>();
        foreach (var r in Screen.resolutions)
            resChoices.Add($"{r.width}×{r.height}");
        resChoices.Reverse();
        resolutionDrop.choices = resChoices;

        /* Window-modes                                              */
        windowModeDrop.choices = new() { "ExclusiveFullScreen", "Fullscreen", "Windowed", "Borderless" };

        /* VSync                                                     */
        vSyncDrop.choices = new() { "Off", "Every VBlank", "Half Refresh", "Quarter" };

        /* Anti-aliasing                                             */
        aaDrop.choices = new()
        { "Off", "MSAA 2×", "MSAA 4×", "MSAA 8×", "FXAA" };

        /* Quality presets                                           */
        qualityDrop.choices = new List<string>(QualitySettings.names);

        /* Virtual-joystick                                          */
        vJoyDrop.choices = new() { "None", "Buttons", "Joystick" };
    }

    void ReadStoredSettingsIntoUI()
    {
        beforeChange = SettingsData.Load();     // snapshot

        displayDrop.index    = beforeChange.displayIndex;
        resolutionDrop.value = $"{beforeChange.resolutionWidth}×{beforeChange.resolutionHeight}";
        windowModeDrop.index = ModeToIndex(beforeChange.windowMode);
        vSyncDrop.index      = Mathf.Clamp(beforeChange.vsyncMode, 0, vSyncDrop.choices.Count-1);
        aaDrop.value         = beforeChange.aaMode;
        qualityDrop.index    = Mathf.Clamp(beforeChange.graphicsQuality, 0,
                                           qualityDrop.choices.Count-1);

        camSmoothT.value     = beforeChange.cameraSmoothing;
        vJoyDrop.value       = beforeChange.virtualJoystick;
        sVolumeS.value       = beforeChange.soundVolume * 100f;
        mVolumeS.value       = beforeChange.musicVolume * 100f;
        devModeT.value       = beforeChange.developerMode;
        showFpsT.value       = beforeChange.showFps;
    }
    #endregion


    /* ============================================================ */
    #region Callbacks & UI logic
    /* ============================================================ */

    void RegisterCallbacks()
    {
        /* Change of display could rebuild resolution list later if
           you add true multi-monitor support                       */
        optResetBtn.clicked += OnOptionsResetPressed;
        saveBtn.clicked += OnSavePressed;
        backBtn.clicked += OnBackPressed;
    }

    void OnOptionsResetPressed()
    {
        pending = new SettingsData()
        {
            displayIndex      = 0,
            resolutionWidth   = 640,
            resolutionHeight  = 480,
            windowMode        = FullScreenMode.Windowed,
            vsyncMode         = 1,
            aaMode            = "Off",
            graphicsQuality   = 0,

            cameraSmoothing   = false,
            virtualJoystick   = "None",
            soundVolume       = 1,
            musicVolume       = 1,
            developerMode     = false,
            showFps           = false
        };
    }

    void OnSavePressed()
    {
        pending = BuildSettingsFromUI();

        /* Apply preview for *risky* fields only (res + mode)        */
        if (RiskyChangeExists(beforeChange, pending))
            PreviewRiskyChange(pending);

        /* Start / restart timer if risky settings were changed      */
        if (revertCo != null) {
            StopCoroutine(revertCo);
            Commit();
        } else {
            if (RiskyChangeExists(beforeChange, pending))
                revertCo = StartCoroutine(RevertTimer());
            else
                Commit();      // nothing risky → commit instantly
        }
    }

    void OnBackPressed()
    {
        CancelRevertIfAny();
        ApplySettings?.Invoke();
        gameObject.SetActive(false);      // or load previous screen
    }
    #endregion


    /* ============================================================ */
    #region Build / compare / preview
    /* ============================================================ */

    SettingsData BuildSettingsFromUI()
    {
        string[] resSplit = resolutionDrop.value.Split('×');
        int w = int.Parse(resSplit[0]);
        int h = int.Parse(resSplit[1]);

        return new SettingsData
        {
            displayIndex      = displayDrop.index,
            resolutionWidth   = w,
            resolutionHeight  = h,
            windowMode        = IndexToMode(windowModeDrop.index),
            vsyncMode         = vSyncDrop.index,
            aaMode            = aaDrop.value,
            graphicsQuality   = qualityDrop.index,

            cameraSmoothing   = camSmoothT.value,
            virtualJoystick   = vJoyDrop.value,
            soundVolume       = sVolumeS.value / 100f,
            musicVolume       = mVolumeS.value / 100f,
            developerMode     = devModeT.value,
            showFps           = showFpsT.value
        };
    }

    bool RiskyChangeExists(SettingsData a, SettingsData b)
    {
        return a.resolutionWidth  != b.resolutionWidth  ||
               a.resolutionHeight != b.resolutionHeight ||
               a.windowMode       != b.windowMode;
    }

    void PreviewRiskyChange(SettingsData s)
    {
        Screen.SetResolution(s.resolutionWidth,
                             s.resolutionHeight,
                             s.windowMode,
                             Screen.currentResolution.refreshRateRatio);
    }
    #endregion


    /* ============================================================ */
    #region Revert timer
    /* ============================================================ */

    IEnumerator RevertTimer()
    {
        revertLbl.style.display = DisplayStyle.Flex;
        float timeLeft = revertTimeout;

        while (timeLeft > 0)
        {
            revertLbl.text = $"Save again within {Mathf.CeilToInt(timeLeft)} s or revert";
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;

            /* Second press of SAVE or hitting Return = confirm      */
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Commit();
                yield break;
            }
        }

        // Time-out – roll back
        PreviewRiskyChange(beforeChange);
        CancelRevertIfAny();
    }

    void Commit()
    {
        pending.Save();                  // write to PlayerPrefs
        beforeChange = pending;          // new baseline
        CancelRevertIfAny();
    }

    void CancelRevertIfAny()
    {
        if (revertCo != null) StopCoroutine(revertCo);
        revertCo = null;
        revertLbl.style.display = DisplayStyle.None;
    }
    #endregion


    /* ============================================================ */
    #region Helpers
    /* ============================================================ */
    static int ModeToIndex(FullScreenMode mode) => mode switch
    {
        FullScreenMode.ExclusiveFullScreen => 0,
        FullScreenMode.FullScreenWindow    => 1,   // Borderless
        FullScreenMode.Windowed            => 2,
        FullScreenMode.MaximizedWindow     => 3,
        _                                  => 2    // Windowed
    };

    static FullScreenMode IndexToMode(int idx) => idx switch
    {
        0 => FullScreenMode.ExclusiveFullScreen,
        1 => FullScreenMode.FullScreenWindow,
        2 => FullScreenMode.Windowed,
        3 => FullScreenMode.MaximizedWindow,
        _ => FullScreenMode.Windowed
    };
    #endregion
}
