// // SettingsMenu.cs
// using UnityEngine;
// using UnityEngine.UIElements;
// using UnityEngine.Audio;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
//
// public class SettingsMenu : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private UIDocument settingsDocument;
//     [SerializeField] private AudioMixer audioMixer;
//     [SerializeField] private UniversalRenderPipelineAsset urpAsset;
//
//     [Header("Settings Data")]
//     [SerializeField] private SettingsData settingsData;
//
//     // UI Elements
//     private VisualElement settingsContainer;
//     private Button backButton;
//     private Button generalTabButton;
//     private Button graphicsTabButton;
//     private Button audioTabButton;
//     private Button controlsTabButton;
//     private Button platformTabButton;
//     private Button applyButton;
//     private Button cancelButton;
//
//     // Panels
//     private VisualElement generalPanel;
//     private VisualElement graphicsPanel;
//     private VisualElement audioPanel;
//     private VisualElement controlsPanel;
//     private VisualElement platformPanel;
//
//     // General Settings
//     private DropdownField languageDropdown;
//     private Toggle subtitlesToggle;
//     private Slider subtitleSizeSlider;
//     private Slider brightnessSlider;
//     private Toggle tutorialsToggle;
//     private Toggle notificationsToggle;
//     private Toggle dataCollectionToggle;
//
//     // Graphics Settings
//     private DropdownField graphicsQualityDropdown;
//     private DropdownField resolutionDropdown;
//     private DropdownField fullscreenDropdown;
//     private Toggle vsyncToggle;
//     private DropdownField antiAliasingDropdown;
//     private DropdownField textureQualityDropdown;
//     private DropdownField shadowQualityDropdown;
//     private Slider shadowDistanceSlider;
//     private Slider renderScaleSlider;
//     private Toggle motionBlurToggle;
//     private Toggle bloomToggle;
//     private Toggle dofToggle;
//     private Toggle aoToggle;
//     private Toggle ssrToggle;
//     private Slider fpsLimitSlider;
//
//     // Audio Settings
//     private Slider masterVolumeSlider;
//     private Slider musicVolumeSlider;
//     private Slider sfxVolumeSlider;
//     private Slider voiceVolumeSlider;
//     private Slider uiVolumeSlider;
//     private DropdownField dynamicRangeDropdown;
//     private DropdownField audioDeviceDropdown;
//     private Toggle hrtfToggle;
//     private Toggle audioDuckingToggle;
//     private Toggle subtitleAudioIndicatorsToggle;
//
//     // Controls Settings
//     private DropdownField inputDeviceDropdown;
//     private DropdownField controlSchemeDropdown;
//     private Slider mouseSensitivitySlider;
//     private Slider controllerSensitivitySlider;
//     private Toggle invertYToggle;
//     private Toggle invertXToggle;
//     private Toggle vibrationToggle;
//     private Slider vibrationIntensitySlider;
//     private Slider controllerDeadzoneSlider;
//     private Toggle mouseAccelerationToggle;
//     private Toggle mouseSmoothingToggle;
//     private Toggle touchControlsToggle;
//     private Slider touchControlSizeSlider;
//     private Slider touchControlOpacitySlider;
//     private Button keyBindingsButton;
//
//     // Platform Settings
//     private VisualElement windowsSettings;
//     private VisualElement macSettings;
//     private VisualElement linuxSettings;
//     private VisualElement androidSettings;
//     private VisualElement webglSettings;
//
//     // Platform Specific Settings
//     private Toggle dpiScalingToggle;
//     private Toggle hardwareAccelerationToggle;
//     private Toggle gameBarToggle;
//     private DropdownField retinaModeDropdown;
//     private Toggle metalValidationToggle;
//     private Toggle vulkanValidationToggle;
//     private DropdownField displayServerDropdown;
//     private Toggle batteryOptimizationToggle;
//     private DropdownField performanceModeDropdown;
//     private DropdownField installLocationDropdown;
//     private Toggle browserNotificationsToggle;
//     private DropdownField webglCompressionDropdown;
//     private Slider memorySizeSlider;
//
//     // Current settings
//     private SettingsData currentSettings;
//     private SettingsData originalSettings;
//
//     // Platform detection
//     private RuntimePlatform currentPlatform;
//
//     private void OnEnable()
//     {
//         // Cache the root element
//         settingsContainer = settingsDocument.rootVisualElement;
//
//         // Cache all UI elements
//         CacheUIElements();
//
//         // Load settings
//         LoadSettings();
//
//         // Initialize UI
//         InitializeUI();
//
//         // Set up event listeners
//         SetupEventListeners();
//
//         // Detect platform and show appropriate settings
//         DetectPlatform();
//     }
//
//     private void CacheUIElements()
//     {
//         // Buttons
//         backButton = settingsContainer.Q<Button>("back-button");
//         generalTabButton = settingsContainer.Q<Button>("general-tab");
//         graphicsTabButton = settingsContainer.Q<Button>("graphics-tab");
//         audioTabButton = settingsContainer.Q<Button>("audio-tab");
//         controlsTabButton = settingsContainer.Q<Button>("controls-tab");
//         platformTabButton = settingsContainer.Q<Button>("platform-tab");
//         applyButton = settingsContainer.Q<Button>("apply-button");
//         cancelButton = settingsContainer.Q<Button>("cancel-button");
//
//         // Panels
//         generalPanel = settingsContainer.Q<VisualElement>("general-panel");
//         graphicsPanel = settingsContainer.Q<VisualElement>("graphics-panel");
//         audioPanel = settingsContainer.Q<VisualElement>("audio-panel");
//         controlsPanel = settingsContainer.Q<VisualElement>("controls-panel");
//         platformPanel = settingsContainer.Q<VisualElement>("platform-panel");
//
//         // General Settings
//         languageDropdown = settingsContainer.Q<DropdownField>("language-dropdown");
//         subtitlesToggle = settingsContainer.Q<Toggle>("subtitles-toggle");
//         subtitleSizeSlider = settingsContainer.Q<Slider>("subtitle-size-slider");
//         brightnessSlider = settingsContainer.Q<Slider>("brightness-slider");
//         tutorialsToggle = settingsContainer.Q<Toggle>("tutorials-toggle");
//         notificationsToggle = settingsContainer.Q<Toggle>("notifications-toggle");
//         dataCollectionToggle = settingsContainer.Q<Toggle>("data-collection-toggle");
//
//         // Graphics Settings
//         graphicsQualityDropdown = settingsContainer.Q<DropdownField>("graphics-quality-dropdown");
//         resolutionDropdown = settingsContainer.Q<DropdownField>("resolution-dropdown");
//         fullscreenDropdown = settingsContainer.Q<DropdownField>("fullscreen-dropdown");
//         vsyncToggle = settingsContainer.Q<Toggle>("vsync-toggle");
//         antiAliasingDropdown = settingsContainer.Q<DropdownField>("anti-aliasing-dropdown");
//         textureQualityDropdown = settingsContainer.Q<DropdownField>("texture-quality-dropdown");
//         shadowQualityDropdown = settingsContainer.Q<DropdownField>("shadow-quality-dropdown");
//         shadowDistanceSlider = settingsContainer.Q<Slider>("shadow-distance-slider");
//         renderScaleSlider = settingsContainer.Q<Slider>("render-scale-slider");
//         motionBlurToggle = settingsContainer.Q<Toggle>("motion-blur-toggle");
//         bloomToggle = settingsContainer.Q<Toggle>("bloom-toggle");
//         dofToggle = settingsContainer.Q<Toggle>("dof-toggle");
//         aoToggle = settingsContainer.Q<Toggle>("ao-toggle");
//         ssrToggle = settingsContainer.Q<Toggle>("ssr-toggle");
//         fpsLimitSlider = settingsContainer.Q<Slider>("fps-limit-slider");
//
//         // Audio Settings
//         masterVolumeSlider = settingsContainer.Q<Slider>("master-volume-slider");
//         musicVolumeSlider = settingsContainer.Q<Slider>("music-volume-slider");
//         sfxVolumeSlider = settingsContainer.Q<Slider>("sfx-volume-slider");
//         voiceVolumeSlider = settingsContainer.Q<Slider>("voice-volume-slider");
//         uiVolumeSlider = settingsContainer.Q<Slider>("ui-volume-slider");
//         dynamicRangeDropdown = settingsContainer.Q<DropdownField>("dynamic-range-dropdown");
//         audioDeviceDropdown = settingsContainer.Q<DropdownField>("audio-device-dropdown");
//         hrtfToggle = settingsContainer.Q<Toggle>("hrtf-toggle");
//         audioDuckingToggle = settingsContainer.Q<Toggle>("audio-ducking-toggle");
//         subtitleAudioIndicatorsToggle = settingsContainer.Q<Toggle>("subtitle-audio-indicators-toggle");
//
//         // Controls Settings
//         inputDeviceDropdown = settingsContainer.Q<DropdownField>("input-device-dropdown");
//         controlSchemeDropdown = settingsContainer.Q<DropdownField>("control-scheme-dropdown");
//         mouseSensitivitySlider = settingsContainer.Q<Slider>("mouse-sensitivity-slider");
//         controllerSensitivitySlider = settingsContainer.Q<Slider>("controller-sensitivity-slider");
//         invertYToggle = settingsContainer.Q<Toggle>("invert-y-toggle");
//         invertXToggle = settingsContainer.Q<Toggle>("invert-x-toggle");
//         vibrationToggle = settingsContainer.Q<Toggle>("vibration-toggle");
//         vibrationIntensitySlider = settingsContainer.Q<Slider>("vibration-intensity-slider");
//         controllerDeadzoneSlider = settingsContainer.Q<Slider>("controller-deadzone-slider");
//         mouseAccelerationToggle = settingsContainer.Q<Toggle>("mouse-acceleration-toggle");
//         mouseSmoothingToggle = settingsContainer.Q<Toggle>("mouse-smoothing-toggle");
//         touchControlsToggle = settingsContainer.Q<Toggle>("touch-controls-toggle");
//         touchControlSizeSlider = settingsContainer.Q<Slider>("touch-control-size-slider");
//         touchControlOpacitySlider = settingsContainer.Q<Slider>("touch-control-opacity-slider");
//         keyBindingsButton = settingsContainer.Q<Button>("key-bindings-button");
//
//         // Platform Settings
//         windowsSettings = settingsContainer.Q<VisualElement>("windows-settings");
//         macSettings = settingsContainer.Q<VisualElement>("mac-settings");
//         linuxSettings = settingsContainer.Q<VisualElement>("linux-settings");
//         androidSettings = settingsContainer.Q<VisualElement>("android-settings");
//         webglSettings = settingsContainer.Q<VisualElement>("webgl-settings");
//
//         // Platform Specific Settings
//         dpiScalingToggle = settingsContainer.Q<Toggle>("dpi-scaling-toggle");
//         hardwareAccelerationToggle = settingsContainer.Q<Toggle>("hardware-acceleration-toggle");
//         gameBarToggle = settingsContainer.Q<Toggle>("game-bar-toggle");
//         retinaModeDropdown = settingsContainer.Q<DropdownField>("retina-mode-dropdown");
//         metalValidationToggle = settingsContainer.Q<Toggle>("metal-validation-toggle");
//         vulkanValidationToggle = settingsContainer.Q<Toggle>("vulkan-validation-toggle");
//         displayServerDropdown = settingsContainer.Q<DropdownField>("display-server-dropdown");
//         batteryOptimizationToggle = settingsContainer.Q<Toggle>("battery-optimization-toggle");
//         performanceModeDropdown = settingsContainer.Q<DropdownField>("performance-mode-dropdown");
//         installLocationDropdown = settingsContainer.Q<DropdownField>("install-location-dropdown");
//         browserNotificationsToggle = settingsContainer.Q<Toggle>("browser-notifications-toggle");
//         webglCompressionDropdown = settingsContainer.Q<DropdownField>("webgl-compression-dropdown");
//         memorySizeSlider = settingsContainer.Q<Slider>("memory-size-slider");
//     }
//
//     private void LoadSettings()
//     {
//         // Load settings from PlayerPrefs or create new instance
//         if (PlayerPrefs.HasKey("SettingsData"))
//         {
//             string json = PlayerPrefs.GetString("SettingsData");
//             currentSettings = JsonUtility.FromJson<SettingsData>(json);
//         }
//         else
//         {
//             currentSettings = new SettingsData();
//             currentSettings.InitializeDefaultSettings();
//         }
//
//         // Create a copy of the original settings for cancel functionality
//         originalSettings = new SettingsData();
//         JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(currentSettings), originalSettings);
//     }
//
//     private void InitializeUI()
//     {
//         // Initialize all dropdowns and toggles with current settings
//         InitializeGeneralSettings();
//         InitializeGraphicsSettings();
//         InitializeAudioSettings();
//         InitializeControlsSettings();
//         InitializePlatformSettings();
//     }
//
//     private void InitializeGeneralSettings()
//     {
//         // Language dropdown
//         languageDropdown.choices = new List<string> { "English", "Spanish", "French", "German", "Japanese", "Chinese" };
//         languageDropdown.index = currentSettings.languageIndex;
//
//         // Subtitles toggle
//         subtitlesToggle.value = currentSettings.subtitlesEnabled;
//
//         // Subtitle size slider
//         subtitleSizeSlider.value = currentSettings.subtitleSize;
//
//         // Brightness slider
//         brightnessSlider.value = currentSettings.brightness;
//
//         // Tutorials toggle
//         tutorialsToggle.value = currentSettings.tutorialsEnabled;
//
//         // Notifications toggle
//         notificationsToggle.value = currentSettings.notificationsEnabled;
//
//         // Data collection toggle
//         dataCollectionToggle.value = currentSettings.dataCollectionEnabled;
//     }
//
//     private void InitializeGraphicsSettings()
//     {
//         // Graphics quality dropdown
//         graphicsQualityDropdown.choices = QualitySettings.names.ToList();
//         graphicsQualityDropdown.index = currentSettings.graphicsQuality;
//
//         // Resolution dropdown
//         resolutionDropdown.choices = new List<string>();
//         foreach (var res in Screen.resolutions)
//         {
//             resolutionDropdown.choices.Add($"{res.width} x {res.height} @ {res.refreshRate}Hz");
//         }
//         resolutionDropdown.index = currentSettings.resolutionIndex;
//
//         // Fullscreen dropdown
//         fullscreenDropdown.choices = new List<string> { "Fullscreen", "Windowed", "Borderless Window" };
//         fullscreenDropdown.index = currentSettings.fullscreenMode;
//
//         // VSync toggle
//         vsyncToggle.value = currentSettings.vSyncEnabled;
//
//         // Anti-aliasing dropdown
//         antiAliasingDropdown.choices = new List<string> { "Disabled", "2x", "4x", "8x" };
//         antiAliasingDropdown.index = currentSettings.antiAliasing;
//
//         // Texture quality dropdown
//         textureQualityDropdown.choices = new List<string> { "Low", "Medium", "High", "Ultra" };
//         textureQualityDropdown.index = currentSettings.textureQuality;
//
//         // Shadow quality dropdown
//         shadowQualityDropdown.choices = new List<string> { "Disabled", "Low", "Medium", "High" };
//         shadowQualityDropdown.index = currentSettings.shadowQuality;
//
//         // Shadow distance slider
//         shadowDistanceSlider.value = currentSettings.shadowDistance;
//
//         // Render scale slider
//         renderScaleSlider.value = currentSettings.renderScale;
//
//         // Post-processing toggles
//         motionBlurToggle.value = currentSettings.motionBlurEnabled;
//         bloomToggle.value = currentSettings.bloomEnabled;
//         dofToggle.value = currentSettings.dofEnabled;
//         aoToggle.value = currentSettings.aoEnabled;
//         ssrToggle.value = currentSettings.ssrEnabled;
//
//         // FPS limit slider
//         fpsLimitSlider.value = currentSettings.fpsLimit;
//     }
//
//     private void InitializeAudioSettings()
//     {
//         // Volume sliders
//         masterVolumeSlider.value = currentSettings.masterVolume;
//         musicVolumeSlider.value = currentSettings.musicVolume;
//         sfxVolumeSlider.value = currentSettings.sfxVolume;
//         voiceVolumeSlider.value = currentSettings.voiceVolume;
//         uiVolumeSlider.value = currentSettings.uiVolume;
//
//         // Dynamic range dropdown
//         dynamicRangeDropdown.choices = new List<string> { "Low", "Medium", "High" };
//         dynamicRangeDropdown.index = currentSettings.dynamicRange;
//
//         // Audio device dropdown
//         audioDeviceDropdown.choices = new List<string>();
//         var devices = AudioSettings.GetConfiguration().speakerMode.ToString();
//         audioDeviceDropdown.choices.Add(devices);
//         audioDeviceDropdown.index = 0;
//
//         // Audio toggles
//         hrtfToggle.value = currentSettings.hrtfEnabled;
//         audioDuckingToggle.value = currentSettings.audioDuckingEnabled;
//         subtitleAudioIndicatorsToggle.value = currentSettings.subtitleAudioIndicatorsEnabled;
//     }
//
//     private void InitializeControlsSettings()
//     {
//         // Input device dropdown
//         inputDeviceDropdown.choices = new List<string> { "Keyboard & Mouse", "Gamepad", "Touch" };
//         inputDeviceDropdown.index = currentSettings.inputDevice;
//
//         // Control scheme dropdown
//         controlSchemeDropdown.choices = new List<string> { "Default", "Southpaw", "Custom" };
//         controlSchemeDropdown.index = currentSettings.controlScheme;
//
//         // Sensitivity sliders
//         mouseSensitivitySlider.value = currentSettings.mouseSensitivity;
//         controllerSensitivitySlider.value = currentSettings.controllerSensitivity;
//
//         // Axis inversion toggles
//         invertYToggle.value = currentSettings.invertYAxis;
//         invertXToggle.value = currentSettings.invertXAxis;
//
//         // Vibration settings
//         vibrationToggle.value = currentSettings.vibrationEnabled;
//         vibrationIntensitySlider.value = currentSettings.vibrationIntensity;
//         controllerDeadzoneSlider.value = currentSettings.controllerDeadzone;
//
//         // Mouse settings
//         mouseAccelerationToggle.value = currentSettings.mouseAcceleration;
//         mouseSmoothingToggle.value = currentSettings.mouseSmoothing;
//
//         // Touch settings
//         touchControlsToggle.value = currentSettings.touchControlsEnabled;
//         touchControlSizeSlider.value = currentSettings.touchControlSize;
//         touchControlOpacitySlider.value = currentSettings.touchControlOpacity;
//     }
//
//     private void InitializePlatformSettings()
//     {
//         // Platform specific settings will be initialized based on detected platform
//     }
//
//     private void SetupEventListeners()
//     {
//         // Tab buttons
//         generalTabButton.clicked += () => ShowPanel(generalPanel);
//         graphicsTabButton.clicked += () => ShowPanel(graphicsPanel);
//         audioTabButton.clicked += () => ShowPanel(audioPanel);
//         controlsTabButton.clicked += () => ShowPanel(controlsPanel);
//         platformTabButton.clicked += () => ShowPanel(platformPanel);
//
//         // Back button
//         backButton.clicked += () => gameObject.SetActive(false);
//
//         // Apply button
//         applyButton.clicked += ApplySettings;
//
//         // Cancel button
//         cancelButton.clicked += CancelSettings;
//
//         // Key bindings button
//         keyBindingsButton.clicked += OpenKeyBindingsMenu;
//
//         // General settings
//         languageDropdown.RegisterValueChangedCallback(evt => currentSettings.languageIndex = evt.newValue);
//         subtitlesToggle.RegisterValueChangedCallback(evt => currentSettings.subtitlesEnabled = evt.newValue);
//         subtitleSizeSlider.RegisterValueChangedCallback(evt => currentSettings.subtitleSize = (int)evt.newValue);
//         brightnessSlider.RegisterValueChangedCallback(evt => currentSettings.brightness = (int)evt.newValue);
//         tutorialsToggle.RegisterValueChangedCallback(evt => currentSettings.tutorialsEnabled = evt.newValue);
//         notificationsToggle.RegisterValueChangedCallback(evt => currentSettings.notificationsEnabled = evt.newValue);
//         dataCollectionToggle.RegisterValueChangedCallback(evt => currentSettings.dataCollectionEnabled = evt.newValue);
//
//         // Graphics settings
//         graphicsQualityDropdown.RegisterValueChangedCallback(evt => currentSettings.graphicsQuality = evt.newValue);
//         resolutionDropdown.RegisterValueChangedCallback(evt => currentSettings.resolutionIndex = evt.newValue);
//         fullscreenDropdown.RegisterValueChangedCallback(evt => currentSettings.fullscreenMode = evt.newValue);
//         vsyncToggle.RegisterValueChangedCallback(evt => currentSettings.vSyncEnabled = evt.newValue);
//         antiAliasingDropdown.RegisterValueChangedCallback(evt => currentSettings.antiAliasing = evt.newValue);
//         textureQualityDropdown.RegisterValueChangedCallback(evt => currentSettings.textureQuality = evt.newValue);
//         shadowQualityDropdown.RegisterValueChangedCallback(evt => currentSettings.shadowQuality = evt.newValue);
//         shadowDistanceSlider.RegisterValueChangedCallback(evt => currentSettings.shadowDistance = (int)evt.newValue);
//         renderScaleSlider.RegisterValueChangedCallback(evt => currentSettings.renderScale = (int)evt.newValue);
//         motionBlurToggle.RegisterValueChangedCallback(evt => currentSettings.motionBlurEnabled = evt.newValue);
//         bloomToggle.RegisterValueChangedCallback(evt => currentSettings.bloomEnabled = evt.newValue);
//         dofToggle.RegisterValueChangedCallback(evt => currentSettings.dofEnabled = evt.newValue);
//         aoToggle.RegisterValueChangedCallback(evt => currentSettings.aoEnabled = evt.newValue);
//         ssrToggle.RegisterValueChangedCallback(evt => currentSettings.ssrEnabled = evt.newValue);
//         fpsLimitSlider.RegisterValueChangedCallback(evt => currentSettings.fpsLimit = (int)evt.newValue);
//
//         // Audio settings
//         masterVolumeSlider.RegisterValueChangedCallback(evt => currentSettings.masterVolume = (int)evt.newValue);
//         musicVolumeSlider.RegisterValueChangedCallback(evt => currentSettings.musicVolume = (int)evt.newValue);
//         sfxVolumeSlider.RegisterValueChangedCallback(evt => currentSettings.sfxVolume = (int)evt.newValue);
//         voiceVolumeSlider.RegisterValueChangedCallback(evt => currentSettings.voiceVolume = (int)evt.newValue);
//         uiVolumeSlider.RegisterValueChangedCallback(evt => currentSettings.uiVolume = (int)evt.newValue);
//         dynamicRangeDropdown.RegisterValueChangedCallback(evt => currentSettings.dynamicRange = evt.newValue);
//         hrtfToggle.RegisterValueChangedCallback(evt => currentSettings.hrtfEnabled = evt.newValue);
//         audioDuckingToggle.RegisterValueChangedCallback(evt => currentSettings.audioDuckingEnabled = evt.newValue);
//         subtitleAudioIndicatorsToggle.RegisterValueChangedCallback(evt => currentSettings.subtitleAudioIndicatorsEnabled = evt.newValue);
//
//         // Controls settings
//         inputDeviceDropdown.RegisterValueChangedCallback(evt => currentSettings.inputDevice = evt.newValue);
//         controlSchemeDropdown.RegisterValueChangedCallback(evt => currentSettings.controlScheme = evt.newValue);
//         mouseSensitivitySlider.RegisterValueChangedCallback(evt => currentSettings.mouseSensitivity = evt.newValue);
//         controllerSensitivitySlider.RegisterValueChangedCallback(evt => currentSettings.controllerSensitivity = evt.newValue);
//         invertYToggle.RegisterValueChangedCallback(evt => currentSettings.invertYAxis = evt.newValue);
//         invertXToggle.RegisterValueChangedCallback(evt => currentSettings.invertXAxis = evt.newValue);
//         vibrationToggle.RegisterValueChangedCallback(evt => currentSettings.vibrationEnabled = evt.newValue);
//         vibrationIntensitySlider.RegisterValueChangedCallback(evt => currentSettings.vibrationIntensity = (int)evt.newValue);
//         controllerDeadzoneSlider.RegisterValueChangedCallback(evt => currentSettings.controllerDeadzone = evt.newValue);
//         mouseAccelerationToggle.RegisterValueChangedCallback(evt => currentSettings.mouseAcceleration = evt.newValue);
//         mouseSmoothingToggle.RegisterValueChangedCallback(evt => currentSettings.mouseSmoothing = evt.newValue);
//         touchControlsToggle.RegisterValueChangedCallback(evt => currentSettings.touchControlsEnabled = evt.newValue);
//         touchControlSizeSlider.RegisterValueChangedCallback(evt => currentSettings.touchControlSize = evt.newValue);
//         touchControlOpacitySlider.RegisterValueChangedCallback(evt => currentSettings.touchControlOpacity = evt.newValue);
//
//         // Platform settings
//         dpiScalingToggle.RegisterValueChangedCallback(evt => currentSettings.dpiScalingEnabled = evt.newValue);
//         hardwareAccelerationToggle.RegisterValueChangedCallback(evt => currentSettings.hardwareAccelerationEnabled = evt.newValue);
//         gameBarToggle.RegisterValueChangedCallback(evt => currentSettings.gameBarIntegrationEnabled = evt.newValue);
//         retinaModeDropdown.RegisterValueChangedCallback(evt => currentSettings.retinaDisplayMode = evt.newValue);
//         metalValidationToggle.RegisterValueChangedCallback(evt => currentSettings.metalApiValidationEnabled = evt.newValue);
//         vulkanValidationToggle.RegisterValueChangedCallback(evt => currentSettings.vulkanValidationEnabled = evt.newValue);
//         displayServerDropdown.RegisterValueChangedCallback(evt => currentSettings.displayServer = evt.newValue);
//         batteryOptimizationToggle.RegisterValueChangedCallback(evt => currentSettings.batteryOptimizationEnabled = evt.newValue);
//         performanceModeDropdown.RegisterValueChangedCallback(evt => currentSettings.performanceMode = evt.newValue);
//         installLocationDropdown.RegisterValueChangedCallback(evt => currentSettings.installLocation = evt.newValue);
//         browserNotificationsToggle.RegisterValueChangedCallback(evt => currentSettings.browserNotificationsEnabled = evt.newValue);
//         webglCompressionDropdown.RegisterValueChangedCallback(evt => currentSettings.webglCompression = evt.newValue);
//         memorySizeSlider.RegisterValueChangedCallback(evt => currentSettings.memorySize = (int)evt.newValue);
//     }
//
//     private void ShowPanel(VisualElement panelToShow)
//     {
//         // Hide all panels
//         generalPanel.AddToClassList("hidden");
//         graphicsPanel.AddToClassList("hidden");
//         audioPanel.AddToClassList("hidden");
//         controlsPanel.AddToClassList("hidden");
//         platformPanel.AddToClassList("hidden");
//
//         // Remove hidden class from the panel to show
//         panelToShow.RemoveFromClassList("hidden");
//
//         // Update active tab
//         generalTabButton.RemoveFromClassList("active-tab");
//         graphicsTabButton.RemoveFromClassList("active-tab");
//         audioTabButton.RemoveFromClassList("active-tab");
//         controlsTabButton.RemoveFromClassList("active-tab");
//         platformTabButton.RemoveFromClassList("active-tab");
//
//         if (panelToShow == generalPanel) generalTabButton.AddToClassList("active-tab");
//         else if (panelToShow == graphicsPanel) graphicsTabButton.AddToClassList("active-tab");
//         else if (panelToShow == audioPanel) audioTabButton.AddToClassList("active-tab");
//         else if (panelToShow == controlsPanel) controlsTabButton.AddToClassList("active-tab");
//         else if (panelToShow == platformPanel) platformTabButton.AddToClassList("active-tab");
//     }
//
//     private void DetectPlatform()
//     {
//         currentPlatform = Application.platform;
//
//         // Hide all platform specific settings
//         windowsSettings.AddToClassList("hidden");
//         macSettings.AddToClassList("hidden");
//         linuxSettings.AddToClassList("hidden");
//         androidSettings.AddToClassList("hidden");
//         webglSettings.AddToClassList("hidden");
//
//         // Show appropriate platform settings
//         switch (currentPlatform)
//         {
//             case RuntimePlatform.WindowsPlayer:
//             case RuntimePlatform.WindowsEditor:
//                 windowsSettings.RemoveFromClassList("hidden");
//                 break;
//
//             case RuntimePlatform.OSXPlayer:
//             case RuntimePlatform.OSXEditor:
//                 macSettings.RemoveFromClassList("hidden");
//                 break;
//
//             case RuntimePlatform.LinuxPlayer:
//             case RuntimePlatform.LinuxEditor:
//                 linuxSettings.RemoveFromClassList("hidden");
//                 break;
//
//             case RuntimePlatform.Android:
//                 androidSettings.RemoveFromClassList("hidden");
//                 break;
//
//             case RuntimePlatform.WebGLPlayer:
//                 webglSettings.RemoveFromClassList("hidden");
//                 break;
//         }
//     }
//
//     private void ApplySettings()
//     {
//         // Apply graphics settings
//         ApplyGraphicsSettings();
//
//         // Apply audio settings
//         ApplyAudioSettings();
//
//         // Apply control settings
//         ApplyControlSettings();
//
//         // Apply platform specific settings
//         ApplyPlatformSettings();
//
//         // Save settings to PlayerPrefs
//         string json = JsonUtility.ToJson(currentSettings);
//         PlayerPrefs.SetString("SettingsData", json);
//         PlayerPrefs.Save();
//
//         // Notify other systems that settings have changed
//         SettingsChanged?.Invoke(currentSettings);
//
//         // Close the settings menu
//         gameObject.SetActive(false);
//     }
//
//     private void ApplyGraphicsSettings()
//     {
//         // Set quality level
//         QualitySettings.SetQualityLevel(currentSettings.graphicsQuality);
//
//         // Set resolution
//         if (currentSettings.resolutionIndex < Screen.resolutions.Length)
//         {
//             Resolution resolution = Screen.resolutions[currentSettings.resolutionIndex];
//             Screen.SetResolution(resolution.width, resolution.height, GetFullscreenMode(currentSettings.fullscreenMode), resolution.refreshRate);
//         }
//
//         // Set VSync
//         QualitySettings.vSyncCount = currentSettings.vSyncEnabled ? 1 : 0;
//
//         // Set anti-aliasing
//         switch (currentSettings.antiAliasing)
//         {
//             case 0: // Disabled
//                 QualitySettings.antiAliasing = 0;
//                 break;
//             case 1: // 2x
//                 QualitySettings.antiAliasing = 2;
//                 break;
//             case 2: // 4x
//                 QualitySettings.antiAliasing = 4;
//                 break;
//             case 3: // 8x
//                 QualitySettings.antiAliasing = 8;
//                 break;
//         }
//
//         // Set texture quality
//         QualitySettings.globalTextureMipmapLimit = currentSettings.textureQuality;
//
//         // Set shadow quality
//         // This would be handled by the URP asset or custom shader settings
//
//         // Set shadow distance
//         QualitySettings.shadowDistance = currentSettings.shadowDistance;
//
//         // Set render scale
//         // This would be handled by the URP asset
//
//         // Set post-processing effects
//         // These would be toggled in the URP asset or post-processing volume
//
//         // Set FPS limit
//         Application.targetFrameRate = currentSettings.fpsLimit;
//     }
//
//     private void ApplyAudioSettings()
//     {
//         // Set audio mixer volumes
//         audioMixer.SetFloat("MasterVolume", Mathf.Log10(currentSettings.masterVolume / 100f) * 20);
//         audioMixer.SetFloat("MusicVolume", Mathf.Log10(currentSettings.musicVolume / 100f) * 20);
//         audioMixer.SetFloat("SFXVolume", Mathf.Log10(currentSettings.sfxVolume / 100f) * 20);
//         audioMixer.SetFloat("VoiceVolume", Mathf.Log10(currentSettings.voiceVolume / 100f) * 20);
//         audioMixer.SetFloat("UIVolume", Mathf.Log10(currentSettings.uiVolume / 100f) * 20);
//
//         // Set other audio settings
//         // These would be handled by the audio system
//     }
//
//     private void ApplyControlSettings()
//     {
//         // Apply input device settings
//         // This would be handled by the input system
//
//         // Apply control scheme
//         // This would be handled by the input system
//
//         // Apply sensitivity settings
//         // These would be handled by the input system
//
//         // Apply vibration settings
//         // These would be handled by the input system
//
//         // Apply touch control settings
//         // These would be handled by the touch input system
//     }
//
//     private void ApplyPlatformSettings()
//     {
//         // Apply platform specific settings
//         switch (currentPlatform)
//         {
//             case RuntimePlatform.WindowsPlayer:
//             case RuntimePlatform.WindowsEditor:
//                 // Apply Windows specific settings
//                 break;
//
//             case RuntimePlatform.OSXPlayer:
//             case RuntimePlatform.OSXEditor:
//                 // Apply Mac specific settings
//                 break;
//
//             case RuntimePlatform.LinuxPlayer:
//             case RuntimePlatform.LinuxEditor:
//                 // Apply Linux specific settings
//                 break;
//
//             case RuntimePlatform.Android:
//                 // Apply Android specific settings
//                 break;
//
//             case RuntimePlatform.WebGLPlayer:
//                 // Apply WebGL specific settings
//                 break;
//         }
//     }
//
//     private FullScreenMode GetFullscreenMode(int index)
//     {
//         switch (index)
//         {
//             case 0: return FullScreenMode.ExclusiveFullScreen;
//             case 1: return FullScreenMode.Windowed;
//             case 2: return FullScreenMode.FullScreenWindow;
//             default: return FullScreenMode.ExclusiveFullScreen;
//         }
//     }
//
//     private void CancelSettings()
//     {
//         // Revert to original settings
//         currentSettings = new SettingsData();
//         JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(originalSettings), currentSettings);
//
//         // Reinitialize UI with original settings
//         InitializeUI();
//
//         // Close the settings menu
//         gameObject.SetActive(false);
//     }
//
//     private void OpenKeyBindingsMenu()
//     {
//         // Open the key bindings menu
//         // This would be handled by a separate system
//     }
//
//     // Event for when settings are changed
//     public delegate void SettingsChangedEvent(SettingsData newSettings);
//     public static event SettingsChangedEvent SettingsChanged;
// }
//
// // Settings data class
// [System.Serializable]
// public class SettingsData
// {
//     // General Settings
//     public int languageIndex;
//     public bool subtitlesEnabled;
//     public int subtitleSize;
//     public int brightness;
//     public bool tutorialsEnabled;
//     public bool notificationsEnabled;
//     public bool dataCollectionEnabled;
//
//     // Graphics Settings
//     public int graphicsQuality;
//     public int resolutionIndex;
//     public int fullscreenMode;
//     public bool vSyncEnabled;
//     public int antiAliasing;
//     public int textureQuality;
//     public int shadowQuality;
//     public int shadowDistance;
//     public int renderScale;
//     public bool motionBlurEnabled;
//     public bool bloomEnabled;
//     public bool dofEnabled;
//     public bool aoEnabled;
//     public bool ssrEnabled;
//     public int fpsLimit;
//
//     // Audio Settings
//     public int masterVolume;
//     public int musicVolume;
//     public int sfxVolume;
//     public int voiceVolume;
//     public int uiVolume;
//     public int dynamicRange;
//     public bool hrtfEnabled;
//     public bool audioDuckingEnabled;
//     public bool subtitleAudioIndicatorsEnabled;
//
//     // Controls Settings
//     public int inputDevice;
//     public int controlScheme;
//     public float mouseSensitivity;
//     public float controllerSensitivity;
//     public bool invertYAxis;
//     public bool invertXAxis;
//     public bool vibrationEnabled;
//     public int vibrationIntensity;
//     public float controllerDeadzone;
//     public bool mouseAcceleration;
//     public bool mouseSmoothing;
//     public bool touchControlsEnabled;
//     public float touchControlSize;
//     public float touchControlOpacity;
//
//     // Platform Specific Settings
//     public bool dpiScalingEnabled;
//     public bool hardwareAccelerationEnabled;
//     public bool gameBarIntegrationEnabled;
//     public int retinaDisplayMode;
//     public bool metalApiValidationEnabled;
//     public bool vulkanValidationEnabled;
//     public int displayServer;
//     public bool batteryOptimizationEnabled;
//     public int performanceMode;
//     public int installLocation;
//     public bool browserNotificationsEnabled;
//     public int webglCompression;
//     public int memorySize;
//
//     public void InitializeDefaultSettings()
//     {
//         // General Settings
//         languageIndex = 0;
//         subtitlesEnabled = true;
//         subtitleSize = 12;
//         brightness = 50;
//         tutorialsEnabled = true;
//         notificationsEnabled = true;
//         dataCollectionEnabled = true;
//
//         // Graphics Settings
//         graphicsQuality = QualitySettings.names.Length - 1; // Highest quality by default
//         resolutionIndex = Screen.resolutions.Length - 1; // Highest resolution by default
//         fullscreenMode = 0; // Fullscreen
//         vSyncEnabled = true;
//         antiAliasing = 1; // 2x
//         textureQuality = 0; // High
//         shadowQuality = 2; // Medium
//         shadowDistance = 50;
//         renderScale = 100;
//         motionBlurEnabled = true;
//         bloomEnabled = true;
//         dofEnabled = true;
//         aoEnabled = true;
//         ssrEnabled = true;
//         fpsLimit = 60;
//
//         // Audio Settings
//         masterVolume = 100;
//         musicVolume = 80;
//         sfxVolume = 90;
//         voiceVolume = 100;
//         uiVolume = 100;
//         dynamicRange = 1; // Medium
//         hrtfEnabled = true;
//         audioDuckingEnabled = true;
//         subtitleAudioIndicatorsEnabled = true;
//
//         // Controls Settings
//         inputDevice = 0; // Keyboard & Mouse
//         controlScheme = 0; // Default
//         mouseSensitivity = 1.0f;
//         controllerSensitivity = 1.0f;
//         invertYAxis = false;
//         invertXAxis = false;
//         vibrationEnabled = true;
//         vibrationIntensity = 100;
//         controllerDeadzone = 0.1f;
//         mouseAcceleration = false;
//         mouseSmoothing = true;
//         touchControlsEnabled = true;
//         touchControlSize = 1.0f;
//         touchControlOpacity = 0.8f;
//
//         // Platform Specific Settings
//         dpiScalingEnabled = true;
//         hardwareAccelerationEnabled = true;
//         gameBarIntegrationEnabled = true;
//         retinaDisplayMode = 0;
//         metalApiValidationEnabled = false;
//         vulkanValidationEnabled = false;
//         displayServer = 0;
//         batteryOptimizationEnabled = true;
//         performanceMode = 1;
//         installLocation = 0;
//         browserNotificationsEnabled = true;
//         webglCompression = 0;
//         memorySize = 1024;
//     }
// }
