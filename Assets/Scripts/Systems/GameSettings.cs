using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GameSettings
{
    // --- Display Settings ---
    public static FullScreenMode WindowMode { get; private set; }
    public static Resolution CurrentResolution { get; private set; } // Stores width, height, refreshRateRatio
    public static int VSyncCount { get; private set; } // 0 = Off, 1 = On (60fps), 2 = On (120fps)

    // --- Graphics Settings ---
    public static int QualityLevel { get; private set; } // Index of QualitySettings.names
    public static int AntiAliasing { get; private set; } // 0, 2, 4, 8

    // --- Gameplay Settings ---
    public static bool CameraSmoothingEnabled { get; private set; }
    public static string VirtualJoystickMode { get; private set; } // e.g., "Off", "Thumbstick", "DPad"

    // --- Audio Settings ---
    public static float SoundVolume { get; private set; } // 0.0 to 1.0
    public static float MusicVolume { get; private set; } // 0.0 to 1.0

    // --- Debug/Developer Settings ---
    public static bool DeveloperModeEnabled { get; private set; }
    public static bool ShowFPS { get; private set; }

    // Constants for PlayerPrefs keys
    private const string PLAYERPREF_WINDOWMODE = "WindowMode";
    private const string PLAYERPREF_RESOLUTION_WIDTH = "ResWidth";
    private const string PLAYERPREF_RESOLUTION_HEIGHT = "ResHeight";
    private const string PLAYERPREF_RESOLUTION_REFRESHRATE = "ResRefreshRate"; // Unity 2021+ uses RefreshRateRatio
    private const string PLAYERPREF_VSYNC = "VSync";
    private const string PLAYERPREF_QUALITYLEVEL = "QualityLevel";
    private const string PLAYERPREF_AA = "AA";
    private const string PLAYERPREF_CAMSMOOTH = "CameraSmoothing";
    private const string PLAYERPREF_VIRTJOY = "VirtualJoystickMode";
    private const string PLAYERPREF_SOUNDVOLUME = "SoundVolume";
    private const string PLAYERPREF_MUSICVOLUME = "MusicVolume";
    private const string PLAYERPREF_DEVMODE = "DeveloperMode";
    private const string PLAYERPREF_SHOWFPS = "ShowFPS";


    // Load settings from PlayerPrefs or use current Unity/default values
    public static void LoadSettings()
    {
        // Default values usually come from Unity's current state or sensible defaults
        WindowMode = (FullScreenMode)PlayerPrefs.GetInt(PLAYERPREF_WINDOWMODE, (int)Screen.fullScreenMode);

        // Resolution: PlayerPrefs store width/height, RefreshRateRatio needs to be re-found
        int resWidth = PlayerPrefs.GetInt(PLAYERPREF_RESOLUTION_WIDTH, Screen.currentResolution.width);
        int resHeight = PlayerPrefs.GetInt(PLAYERPREF_RESOLUTION_HEIGHT, Screen.currentResolution.height);

        // Find a matching resolution with the stored width/height (or closest)
        CurrentResolution = Screen.resolutions
            .FirstOrDefault(res => res.width == resWidth && res.height == resHeight);

        if (CurrentResolution.width == 0) // No matching resolution found, default to current
        {
            CurrentResolution = Screen.currentResolution;
        }

        VSyncCount = PlayerPrefs.GetInt(PLAYERPREF_VSYNC, QualitySettings.vSyncCount);
        QualityLevel = PlayerPrefs.GetInt(PLAYERPREF_QUALITYLEVEL, QualitySettings.GetQualityLevel());
        AntiAliasing = PlayerPrefs.GetInt(PLAYERPREF_AA, QualitySettings.antiAliasing);
        CameraSmoothingEnabled = PlayerPrefs.GetInt(PLAYERPREF_CAMSMOOTH, 1) == 1; // 1 for true, 0 for false
        VirtualJoystickMode = PlayerPrefs.GetString(PLAYERPREF_VIRTJOY, "Off");
        SoundVolume = PlayerPrefs.GetFloat(PLAYERPREF_SOUNDVOLUME, 0.75f);
        MusicVolume = PlayerPrefs.GetFloat(PLAYERPREF_MUSICVOLUME, 0.5f);
        DeveloperModeEnabled = PlayerPrefs.GetInt(PLAYERPREF_DEVMODE, 0) == 1;
        ShowFPS = PlayerPrefs.GetInt(PLAYERPREF_SHOWFPS, 0) == 1;

        Debug.Log("Game Settings Loaded.");
    }

    // Apply the loaded settings to Unity's system
    public static void ApplySettings()
    {
        Screen.SetResolution(CurrentResolution.width, CurrentResolution.height, WindowMode, CurrentResolution.refreshRateRatio);
        QualitySettings.vSyncCount = VSyncCount;
        QualitySettings.SetQualityLevel(QualityLevel, true); // applyExpensiveChanges: true
        QualitySettings.antiAliasing = AntiAliasing;

        AudioListener.volume = SoundVolume; // Applies to all audio if no mixer is used

        // Other settings like CameraSmoothingEnabled, VirtualJoystickMode, DeveloperModeEnabled, ShowFPS
        // would be applied by game logic that checks these static properties.
        Debug.Log("Game Settings Applied.");
    }

    // Update methods (used by UI to change values)
    public static void SetWindowMode(FullScreenMode mode) { WindowMode = mode; }
    public static void SetResolution(Resolution res) { CurrentResolution = res; }
    public static void SetVSyncCount(int count) { VSyncCount = count; }
    public static void SetQualityLevel(int level) { QualityLevel = level; }
    public static void SetAntiAliasing(int aa) { AntiAliasing = aa; }
    public static void SetCameraSmoothing(bool enabled) { CameraSmoothingEnabled = enabled; }
    public static void SetVirtualJoystickMode(string mode) { VirtualJoystickMode = mode; }
    public static void SetSoundVolume(float volume) { SoundVolume = volume; }
    public static void SetMusicVolume(float volume) { MusicVolume = volume; }
    public static void SetDeveloperMode(bool enabled) { DeveloperModeEnabled = enabled; }
    public static void SetShowFPS(bool enabled) { ShowFPS = enabled; }

    // Save current settings to PlayerPrefs
    public static void SaveSettings()
    {
        PlayerPrefs.SetInt(PLAYERPREF_WINDOWMODE, (int)WindowMode);
        PlayerPrefs.SetInt(PLAYERPREF_RESOLUTION_WIDTH, CurrentResolution.width);
        PlayerPrefs.SetInt(PLAYERPREF_RESOLUTION_HEIGHT, CurrentResolution.height);
        // PlayerPrefs.SetFloat(PLAYERPREF_RESOLUTION_REFRESHRATE, (float)CurrentResolution.refreshRateRatio.value); // Unity 2021+
        PlayerPrefs.SetInt(PLAYERPREF_VSYNC, VSyncCount);
        PlayerPrefs.SetInt(PLAYERPREF_QUALITYLEVEL, QualityLevel);
        PlayerPrefs.SetInt(PLAYERPREF_AA, AntiAliasing);
        PlayerPrefs.SetInt(PLAYERPREF_CAMSMOOTH, CameraSmoothingEnabled ? 1 : 0);
        PlayerPrefs.SetString(PLAYERPREF_VIRTJOY, VirtualJoystickMode);
        PlayerPrefs.SetFloat(PLAYERPREF_SOUNDVOLUME, SoundVolume);
        PlayerPrefs.SetFloat(PLAYERPREF_MUSICVOLUME, MusicVolume);
        PlayerPrefs.SetInt(PLAYERPREF_DEVMODE, DeveloperModeEnabled ? 1 : 0);
        PlayerPrefs.SetInt(PLAYERPREF_SHOWFPS, ShowFPS ? 1 : 0);
        PlayerPrefs.Save(); // Ensure changes are written to disk
        Debug.Log("Game Settings Saved.");
    }

    // Static constructor or method to ensure settings are loaded at startup
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeOnLoad()
    {
        LoadSettings();
        ApplySettings();
    }
}
