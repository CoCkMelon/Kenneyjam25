using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public int  displayIndex;
    public int  resolutionWidth;
    public int  resolutionHeight;
    public FullScreenMode windowMode;
    public int  vsyncMode;
    public string aaMode;
    public int  graphicsQuality;

    public bool cameraSmoothing;
    public string virtualJoystick;
    public float soundVolume;
    public float musicVolume;
    public bool developerMode;
    public bool showFps;

    /* ---------- Helpers ---------- */
    public static SettingsData Load()
    {
        return new SettingsData
        {
            displayIndex      = PlayerPrefs.GetInt   ("DisplayIndex", 0),
            resolutionWidth   = PlayerPrefs.GetInt   ("ResolutionWidth",  Screen.currentResolution.width),
            resolutionHeight  = PlayerPrefs.GetInt   ("ResolutionHeight", Screen.currentResolution.height),
            windowMode        = (FullScreenMode)PlayerPrefs.GetInt ("WindowMode",
                                           (int)FullScreenMode.ExclusiveFullScreen),
            vsyncMode         = PlayerPrefs.GetInt   ("VSyncMode", 1),
            aaMode            = PlayerPrefs.GetString("AAMode",   "Off"),
            graphicsQuality   = PlayerPrefs.GetInt   ("GraphicsQuality", 2),

            cameraSmoothing   = PlayerPrefs.GetInt   ("CameraSmoothing", 1) == 1,
            virtualJoystick   = PlayerPrefs.GetString("VirtualJoystick", "None"),
            soundVolume       = PlayerPrefs.GetFloat ("SoundVolume", 1f),
            musicVolume       = PlayerPrefs.GetFloat ("MusicVolume", 1f),
            developerMode     = PlayerPrefs.GetInt   ("DeveloperMode", 0) == 1,
            showFps           = PlayerPrefs.GetInt   ("ShowFPS", 0)       == 1
        };
    }

    public void Save()
    {
        PlayerPrefs.SetInt   ("DisplayIndex",    displayIndex);
        PlayerPrefs.SetInt   ("ResolutionWidth", resolutionWidth);
        PlayerPrefs.SetInt   ("ResolutionHeight",resolutionHeight);
        PlayerPrefs.SetInt   ("WindowMode",      (int)windowMode);
        PlayerPrefs.SetInt   ("VSyncMode",       vsyncMode);
        PlayerPrefs.SetString("AAMode",          aaMode);
        PlayerPrefs.SetInt   ("GraphicsQuality", graphicsQuality);

        PlayerPrefs.SetInt   ("CameraSmoothing", cameraSmoothing ? 1 : 0);
        PlayerPrefs.SetString("VirtualJoystick", virtualJoystick);
        PlayerPrefs.SetFloat ("SoundVolume",     soundVolume);
        PlayerPrefs.SetFloat ("MusicVolume",     musicVolume);
        PlayerPrefs.SetInt   ("DeveloperMode",   developerMode ? 1 : 0);
        PlayerPrefs.SetInt   ("ShowFPS",         showFps ? 1 : 0);

        PlayerPrefs.Save();
    }
}
