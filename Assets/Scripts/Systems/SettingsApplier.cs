using System;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-90)]      // run early – before your gameplay scripts
public class SettingsApplier : MonoBehaviour
{
    [Header("Audio (optional)")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string  sfxParam   = "SoundVolume";
    [SerializeField] private string  musicParam = "MusicVolume";

    /* ------------- public events so that other systems can hook in ------------- */
    public static event Action<bool>    CameraSmoothingChanged;
    public static event Action<string>  VirtualJoystickChanged;
    public static event Action<bool>    DeveloperModeChanged;
    public static event Action<bool>    ShowFpsChanged;

    [SerializeField] private OptionsMenuController optionsMenuController;
    [SerializeField] private GameHUDController gameHUDController;

    void OnEnable()          // fires both on first load and when you return from pause
    {
        ApplySettings();
    }
    public void ApplySettings() {
        gameObject.SetActive(true);
        Debug.Log("Applying Settings");
        SettingsData s = SettingsData.Load();
        ApplyGraphics(s);
        ApplyAudio(s);
        RaiseToggles(s);
        
        // Update HUD mobile controls visibility
        if (gameHUDController != null)
        {
            gameHUDController.OnSettingsApplied();
        }
    }

    /* ------------------------------------------------------------------------- */
    void ApplyGraphics(SettingsData s)
    {
        // 1. resolution   -------------------------------------------------------
        if (s.resolutionWidth  > 0 &&
            s.resolutionHeight > 0)
        {
            Screen.SetResolution(s.resolutionWidth,
                             s.resolutionHeight,
                             s.windowMode,
                             Screen.currentResolution.refreshRateRatio);
        }
        else
        {
            // safety fallback
            Resolution r = Screen.currentResolution;
            Screen.SetResolution(r.width, r.height, s.windowMode, r.refreshRateRatio);
        }

        // 2. vsync  -------------------------------------------------------------
        QualitySettings.vSyncCount = Mathf.Clamp(s.vsyncMode, 0, 4);

        // 3. AA  ---------------------------------------------------------------
        ApplyAA(s.aaMode);

        // 4. quality  ----------------------------------------------------------
        QualitySettings.SetQualityLevel(
            Mathf.Clamp(s.graphicsQuality, 0, QualitySettings.names.Length-1), true);
    }

    void ApplyAA(string aa)
    {
        if (string.IsNullOrEmpty(aa) || aa.Equals("Off"))
        {
            QualitySettings.antiAliasing = 0;
            return;
        }

        if (aa.StartsWith("MSAA"))
        {
            if(aa == "MSAA 2×") {
                QualitySettings.antiAliasing = 2;
            } else if (aa == "MSAA 4×") {
                QualitySettings.antiAliasing = 4;
            } else if (aa == "MSAA 8×") {
                QualitySettings.antiAliasing = 8;
            }
        }
        else
        {
            QualitySettings.antiAliasing = 0;
            //  Post AA like FXAA / TAA would be enabled in your own post-process pipeline.
            //  Not referenced here to avoid non-existing calls.
        }
    }

    /* ------------------------------------------------------------------------- */
    void ApplyAudio(SettingsData s)
    {
        // linear 0-1 volume stored – we prefer dB mapping if a mixer is provided
        if (masterMixer)
        {
            float sfxDb   = Mathf.Lerp(-80, 0, s.soundVolume);
            float musicDb = Mathf.Lerp(-80, 0, s.musicVolume);
            masterMixer.SetFloat(sfxParam,   sfxDb);
            masterMixer.SetFloat(musicParam, musicDb);
        }
        else
        {
            AudioListener.volume = s.soundVolume;    // simple fallback
        }
    }

    /* ------------------------------------------------------------------------- */
    void RaiseToggles(SettingsData s)
    {
        CameraSmoothingChanged?.Invoke(s.cameraSmoothing);
        VirtualJoystickChanged?.Invoke(s.virtualJoystick);
        DeveloperModeChanged?.Invoke(s.developerMode);
        ShowFpsChanged?.Invoke(s.showFps);
    }
}
