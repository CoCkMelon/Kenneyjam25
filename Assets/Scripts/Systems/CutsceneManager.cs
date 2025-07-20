using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Manages cutscenes, especially the light speed cutscene
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    [Header("Light Speed Cutscene")]
    [SerializeField] private GameObject lightSpeedCutsceneCamera;
    [SerializeField] private ParticleSystem lightSpeedEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lightSpeedMusic;
    [SerializeField] private float cutsceneDuration = 10f;
    
    [Header("Camera Animation")]
    [SerializeField] private AnimationCurve cameraMovementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector3 cameraStartOffset = new Vector3(0, 5, -10);
    [SerializeField] private Vector3 cameraEndOffset = new Vector3(0, 20, -5);
    [SerializeField] private float cameraRotationSpeed = 45f;
    
    [Header("Post-Processing Effects")]
    [SerializeField] private GameObject screenEffects;
    [SerializeField] private float effectIntensity = 1f;
    [SerializeField] private AnimationCurve effectCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("UI Elements")]
    [SerializeField] private GameObject cutsceneUI;
    [SerializeField] private GameObject gameHUD;
    
    [Header("Events")]
    public UnityEvent OnCutsceneStart;
    public UnityEvent OnCutsceneEnd;
    public UnityEvent OnLightSpeedSequenceComplete;
    
    // Private fields
    private Camera mainCamera;
    private Camera cutsceneCamera;
    private bool isCutscenePlaying = false;
    private Transform playerTransform;
    private SleightController sleightController;
    
    void Awake()
    {
        mainCamera = Camera.main;
        
        if (lightSpeedCutsceneCamera != null)
        {
            cutsceneCamera = lightSpeedCutsceneCamera.GetComponent<Camera>();
            if (cutsceneCamera == null)
            {
                cutsceneCamera = lightSpeedCutsceneCamera.AddComponent<Camera>();
            }
        }
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        // Find player references
        sleightController = FindFirstObjectByType<SleightController>();
        if (sleightController != null)
        {
            playerTransform = sleightController.transform;
        }
        
        // Setup initial state
        SetupInitialState();
    }
    
    private void SetupInitialState()
    {
        // Ensure cutscene camera is disabled initially
        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = false;
        }
        
        // Ensure cutscene UI is hidden
        if (cutsceneUI != null)
        {
            cutsceneUI.SetActive(false);
        }
        
        // Ensure screen effects are disabled
        if (screenEffects != null)
        {
            screenEffects.SetActive(false);
        }
    }
    
    public void PlayLightSpeedCutscene()
    {
        if (isCutscenePlaying) return;
        
        StartCoroutine(LightSpeedCutsceneSequence());
    }
    
    private IEnumerator LightSpeedCutsceneSequence()
    {
        isCutscenePlaying = true;
        
        // Notify cutscene start
        OnCutsceneStart?.Invoke();
        
        // Setup cutscene
        SetupCutscene();
        
        // Play the sequence
        yield return StartCoroutine(PlayCutsceneAnimation());
        
        // Cleanup and end cutscene
        EndCutscene();
        
        isCutscenePlaying = false;
        
        // Notify completion
        OnLightSpeedSequenceComplete?.Invoke();
    }
    
    private void SetupCutscene()
    {
        // Disable main camera
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }
        
        // Enable cutscene camera
        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = true;
            
            // Position camera relative to player
            if (playerTransform != null)
            {
                cutsceneCamera.transform.position = playerTransform.position + cameraStartOffset;
                cutsceneCamera.transform.LookAt(playerTransform);
            }
        }
        
        // Hide game HUD
        if (gameHUD != null)
        {
            gameHUD.SetActive(false);
        }
        
        // Show cutscene UI
        if (cutsceneUI != null)
        {
            cutsceneUI.SetActive(true);
        }
        
        // Enable screen effects
        if (screenEffects != null)
        {
            screenEffects.SetActive(true);
        }
        
        // Play light speed particle effect
        if (lightSpeedEffect != null)
        {
            lightSpeedEffect.Play();
        }
        
        // Play cutscene music
        if (audioSource != null && lightSpeedMusic != null)
        {
            audioSource.clip = lightSpeedMusic;
            audioSource.loop = false;
            audioSource.Play();
        }
        
        // Pause game time (optional)
        Time.timeScale = 0.1f; // Slow motion effect
    }
    
    private IEnumerator PlayCutsceneAnimation()
    {
        float elapsedTime = 0f;
        
        Vector3 startPosition = playerTransform != null ? playerTransform.position + cameraStartOffset : Vector3.zero;
        Vector3 endPosition = playerTransform != null ? playerTransform.position + cameraEndOffset : Vector3.zero;
        
        while (elapsedTime < cutsceneDuration)
        {
            float normalizedTime = elapsedTime / cutsceneDuration;
            
            // Animate camera position
            if (cutsceneCamera != null)
            {
                float curveValue = cameraMovementCurve.Evaluate(normalizedTime);
                cutsceneCamera.transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
                
                // Rotate camera around player
                if (playerTransform != null)
                {
                    cutsceneCamera.transform.RotateAround(playerTransform.position, Vector3.up, cameraRotationSpeed * Time.unscaledDeltaTime);
                    cutsceneCamera.transform.LookAt(playerTransform);
                }
            }
            
            // Animate screen effects
            if (screenEffects != null)
            {
                float effectValue = effectCurve.Evaluate(normalizedTime) * effectIntensity;
                // Apply effect intensity to post-processing or material properties
                ApplyScreenEffectIntensity(effectValue);
            }
            
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }
    
    private void ApplyScreenEffectIntensity(float intensity)
    {
        // This would typically interface with post-processing effects
        // For now, we'll just set a simple brightness effect
        if (screenEffects != null)
        {
            Light[] lights = screenEffects.GetComponentsInChildren<Light>();
            foreach (Light light in lights)
            {
                light.intensity = intensity;
            }
        }
    }
    
    private void EndCutscene()
    {
        // Restore main camera
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }
        
        // Disable cutscene camera
        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = false;
        }
        
        // Show game HUD
        if (gameHUD != null)
        {
            gameHUD.SetActive(true);
        }
        
        // Hide cutscene UI
        if (cutsceneUI != null)
        {
            cutsceneUI.SetActive(false);
        }
        
        // Disable screen effects
        if (screenEffects != null)
        {
            screenEffects.SetActive(false);
        }
        
        // Stop particle effects
        if (lightSpeedEffect != null)
        {
            lightSpeedEffect.Stop();
        }
        
        // Restore normal time scale
        Time.timeScale = 1f;
        
        // Notify cutscene end
        OnCutsceneEnd?.Invoke();
        
        // Trigger transition to next scene
        TriggerSceneTransition();
    }
    
    public void SkipCutscene()
    {
        if (isCutscenePlaying)
        {
            StopAllCoroutines();
            EndCutscene();
            isCutscenePlaying = false;
            OnLightSpeedSequenceComplete?.Invoke();
        }
    }
    
    public bool IsCutscenePlaying()
    {
        return isCutscenePlaying;
    }
    
    // Public methods for other cutscenes
    public void PlayCustomCutscene(string cutsceneName)
    {
        Debug.Log($"Playing custom cutscene: {cutsceneName}");
        // Implement custom cutscene logic here
    }
    
    public void SetCutsceneDuration(float duration)
    {
        cutsceneDuration = duration;
    }
    
    public void SetCameraOffsets(Vector3 startOffset, Vector3 endOffset)
    {
        cameraStartOffset = startOffset;
        cameraEndOffset = endOffset;
    }
    
    private void TriggerSceneTransition()
    {
        // Find SceneTransitionManager and trigger appropriate transition
        SceneTransitionManager sceneManager = SceneTransitionManager.Instance;
        if (sceneManager != null)
        {
            // Depending on the current scene, transition to the appropriate next scene
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (currentScene == "SleighSpeed")
            {
                // After FTL cutscene in sleigh scene, go to puzzle scene
                sceneManager.GoToPuzzleScene();
            }
            else if (currentScene == "FTLCutscene")
            {
                // After dedicated FTL cutscene, go to puzzle scene
                sceneManager.GoToPuzzleScene();
            }
            else
            {
                // Default behavior - go to next logical scene
                Debug.Log("Cutscene completed, but no specific transition defined for scene: " + currentScene);
            }
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager not found!");
        }
    }
}
