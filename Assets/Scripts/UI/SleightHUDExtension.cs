using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Extension to the existing HUD system to display sleight-specific information
/// </summary>
public class SleightHUDExtension : MonoBehaviour
{
    [Header("Sleight UI Elements")]
    [SerializeField] private UIDocument hudDocument;
    
    // Power System UI
    private VisualElement powerBarFill;
    private Label powerText;
    private Label powerPercentageText;
    
    // Speed System UI
    private VisualElement speedBarFill;
    private Label speedText;
    private Label speedPercentageText;
    
    // Combo System UI
    private VisualElement comboContainer;
    private Label comboText;
    private Label comboMultiplierText;
    private VisualElement comboTimerFill;
    
    // Status UI
    private Label statusText;
    private VisualElement lightSpeedIndicator;
    
    // Distance UI
    private Label distanceIndicator;
    private VisualElement worldConnectionIndicator;
    
    // References
    private SleightController sleightController;
    private SleightPowerSystem powerSystem;
    private GameHUDController gameHUD;
    
    [Header("Distance Tracking")]
    [SerializeField] private Transform targetDestination;
    [SerializeField] private bool showDistanceToDestination = true;
    [SerializeField] private bool showDistanceToNearestPowerOrb = false;
    
    void OnEnable()
    {
        // Find sleight controller
        sleightController = FindFirstObjectByType<SleightController>();
        if (sleightController != null)
        {
            powerSystem = sleightController.GetComponent<SleightPowerSystem>();
        }
        
        // Find existing HUD controller
        gameHUD = FindFirstObjectByType<GameHUDController>();
        
        // Initialize UI elements
        InitializeUIElements();
        
        // Subscribe to events
        SubscribeToEvents();
    }
    
    void OnDisable()
    {
        // Unsubscribe from events
        UnsubscribeFromEvents();
    }
    
    private void InitializeUIElements()
    {
        if (hudDocument == null) return;
        
        var root = hudDocument.rootVisualElement;
        
        // Initialize power system UI
        powerBarFill = root.Q<VisualElement>("power-bar-fill");
        powerText = root.Q<Label>("power-text");
        powerPercentageText = root.Q<Label>("power-percentage-text");
        
        // Initialize speed system UI
        speedBarFill = root.Q<VisualElement>("speed-bar-fill");
        speedText = root.Q<Label>("speed-text");
        speedPercentageText = root.Q<Label>("speed-percentage-text");
        
        // Initialize combo system UI
        comboContainer = root.Q<VisualElement>("combo-container");
        comboText = root.Q<Label>("combo-text");
        comboMultiplierText = root.Q<Label>("combo-multiplier-text");
        comboTimerFill = root.Q<VisualElement>("combo-timer-fill");
        
        // Initialize status UI
        statusText = root.Q<Label>("status-text");
        lightSpeedIndicator = root.Q<VisualElement>("light-speed-indicator");
        
        // Initialize distance UI
        distanceIndicator = root.Q<Label>("distance-indicator");
        worldConnectionIndicator = root.Q<VisualElement>("world-connection-indicator");
        
        // Set initial states
        UpdatePowerDisplay();
        UpdateSpeedDisplay();
        UpdateComboDisplay();
        UpdateStatusDisplay();
        UpdateDistanceDisplay();
    }
    
    private void SubscribeToEvents()
    {
        if (sleightController != null)
        {
            sleightController.OnPowerChanged.AddListener(OnPowerChanged);
            sleightController.OnSpeedChanged.AddListener(OnSpeedChanged);
            sleightController.OnLightSpeedReached.AddListener(OnLightSpeedReached);
        }
        
        if (powerSystem != null)
        {
            powerSystem.OnPowerCollected.AddListener(OnPowerCollected);
            powerSystem.OnComboChanged.AddListener(OnComboChanged);
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (sleightController != null)
        {
            sleightController.OnPowerChanged.RemoveListener(OnPowerChanged);
            sleightController.OnSpeedChanged.RemoveListener(OnSpeedChanged);
            sleightController.OnLightSpeedReached.RemoveListener(OnLightSpeedReached);
        }
        
        if (powerSystem != null)
        {
            powerSystem.OnPowerCollected.RemoveListener(OnPowerCollected);
            powerSystem.OnComboChanged.RemoveListener(OnComboChanged);
        }
    }
    
    void Update()
    {
        UpdateComboTimer();
        UpdateStatusDisplay();
        UpdateDistanceDisplay();
    }
    
    private void UpdatePowerDisplay()
    {
        if (powerSystem == null) return;
        
        float powerPercentage = powerSystem.PowerPercentage;
        
        // Update power bar
        if (powerBarFill != null)
        {
            powerBarFill.style.width = Length.Percent(powerPercentage * 100);
            
            // Change color based on power level
            if (powerPercentage > 0.8f)
                powerBarFill.style.backgroundColor = new Color(1f, 0.8f, 0.2f); // Gold
            else if (powerPercentage > 0.6f)
                powerBarFill.style.backgroundColor = new Color(0.8f, 0.4f, 1f); // Purple
            else if (powerPercentage > 0.4f)
                powerBarFill.style.backgroundColor = new Color(0.2f, 0.8f, 1f); // Blue
            else
                powerBarFill.style.backgroundColor = new Color(0.2f, 1f, 0.2f); // Green
        }
        
        // Update power text
        if (powerText != null)
        {
            powerText.text = $"Power: {Mathf.RoundToInt(powerSystem.CurrentPower)}/{Mathf.RoundToInt(powerSystem.MaxPower)}";
        }
        
        // Update power percentage text
        if (powerPercentageText != null)
        {
            powerPercentageText.text = $"{Mathf.RoundToInt(powerPercentage * 100)}%";
        }
    }
    
    private void UpdateSpeedDisplay()
    {
        if (sleightController == null) return;
        
        float speedPercentage = sleightController.SpeedPercentage;
        
        // Update speed bar
        if (speedBarFill != null)
        {
            speedBarFill.style.width = Length.Percent(speedPercentage * 100);
            
            // Change color based on speed level
            if (speedPercentage > 0.9f)
                speedBarFill.style.backgroundColor = new Color(1f, 0.2f, 0.2f); // Red (approaching light speed)
            else if (speedPercentage > 0.7f)
                speedBarFill.style.backgroundColor = new Color(1f, 0.6f, 0.2f); // Orange
            else if (speedPercentage > 0.5f)
                speedBarFill.style.backgroundColor = new Color(1f, 1f, 0.2f); // Yellow
            else
                speedBarFill.style.backgroundColor = new Color(0.2f, 1f, 0.2f); // Green
        }
        
        // Update speed text
        if (speedText != null)
        {
            speedText.text = $"Speed: {Mathf.RoundToInt(sleightController.CurrentSpeed)}";
        }
        
        // Update speed percentage text
        if (speedPercentageText != null)
        {
            speedPercentageText.text = $"{Mathf.RoundToInt(speedPercentage * 100)}%";
        }
    }
    
    private void UpdateComboDisplay()
    {
        if (powerSystem == null) return;
        
        int combo = powerSystem.CurrentCombo;
        
        // Show/hide combo container
        if (comboContainer != null)
        {
            comboContainer.style.display = combo > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        // Update combo text
        if (comboText != null)
        {
            comboText.text = $"Combo: {combo}";
        }
        
        // Update combo multiplier text
        if (comboMultiplierText != null)
        {
            comboMultiplierText.text = $"x{powerSystem.ComboMultiplier:F1}";
        }
    }
    
    private void UpdateComboTimer()
    {
        if (powerSystem == null || comboTimerFill == null) return;
        
        float comboTimeRemaining = powerSystem.GetComboTimeRemaining();
        float comboTimePercentage = comboTimeRemaining / 2f; // Assuming 2 second combo decay time
        
        comboTimerFill.style.width = Length.Percent(comboTimePercentage * 100);
        
        // Change color based on time remaining
        if (comboTimePercentage > 0.6f)
            comboTimerFill.style.backgroundColor = new Color(0.2f, 1f, 0.2f); // Green
        else if (comboTimePercentage > 0.3f)
            comboTimerFill.style.backgroundColor = new Color(1f, 1f, 0.2f); // Yellow
        else
            comboTimerFill.style.backgroundColor = new Color(1f, 0.2f, 0.2f); // Red
    }
    
    private void UpdateStatusDisplay()
    {
        if (sleightController == null) return;
        
        string status = "";
        
        if (sleightController.IsLightSpeedReached)
        {
            status = "LIGHT SPEED REACHED!";
        }
        else if (sleightController.SpeedPercentage > 0.9f)
        {
            status = "Approaching Light Speed...";
        }
        else if (sleightController.IsAutoMoving)
        {
            status = "Auto-Moving";
        }
        else
        {
            status = "Manual Control";
        }
        
        if (statusText != null)
        {
            statusText.text = status;
        }
        
        // Update light speed indicator
        if (lightSpeedIndicator != null)
        {
            lightSpeedIndicator.style.display = sleightController.IsLightSpeedReached ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (sleightController.IsLightSpeedReached)
            {
                // Add pulsing effect
                float pulse = (Mathf.Sin(Time.time * 5f) + 1f) * 0.5f;
                lightSpeedIndicator.style.backgroundColor = Color.Lerp(Color.white, Color.yellow, pulse);
            }
        }
    }
    
    private void UpdateDistanceDisplay()
    {
        if (sleightController == null) return;
        
        float distance = 0f;
        bool hasValidDistance = false;
        
        // Calculate distance based on current settings
        if (showDistanceToDestination && targetDestination != null)
        {
            distance = CalculateDistanceToTarget(targetDestination);
            hasValidDistance = true;
        }
        else if (showDistanceToNearestPowerOrb)
        {
            Transform nearestOrb = FindNearestPowerOrb();
            if (nearestOrb != null)
            {
                distance = CalculateDistanceToTarget(nearestOrb);
                hasValidDistance = true;
            }
        }
        else if (targetDestination != null)
        {
            // Default to destination if available
            distance = CalculateDistanceToTarget(targetDestination);
            hasValidDistance = true;
        }
        
        // Update distance indicator
        if (distanceIndicator != null)
        {
            if (hasValidDistance)
            {
                distanceIndicator.text = FormatDistance(distance);
                distanceIndicator.style.display = DisplayStyle.Flex;
            }
            else
            {
                distanceIndicator.style.display = DisplayStyle.None;
            }
        }
        
        // Update world connection indicator visibility
        if (worldConnectionIndicator != null)
        {
            worldConnectionIndicator.style.display = hasValidDistance ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    private float CalculateDistanceToTarget(Transform target)
    {
        if (target == null || sleightController == null) return 0f;
        
        Vector3 sleightPosition = sleightController.transform.position;
        Vector3 targetPosition = target.position;
        
        // Calculate 3D distance
        float distance = Vector3.Distance(sleightPosition, targetPosition);
        
        return distance;
    }
    
    private Transform FindNearestPowerOrb()
    {
        // Find all power orbs in the scene
        PowerOrb[] powerOrbs = FindObjectsByType<PowerOrb>(FindObjectsSortMode.None);
        
        if (powerOrbs.Length == 0) return null;
        
        Transform nearestOrb = null;
        float nearestDistance = float.MaxValue;
        
        Vector3 sleightPosition = sleightController.transform.position;
        
        foreach (PowerOrb orb in powerOrbs)
        {
            if (orb == null) continue;
            
            float distance = Vector3.Distance(sleightPosition, orb.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestOrb = orb.transform;
            }
        }
        
        return nearestOrb;
    }
    
    private string FormatDistance(float distance)
    {
        if (distance < 1f)
        {
            // Show in centimeters for very close distances
            return $"{Mathf.RoundToInt(distance * 100)}cm";
        }
        else if (distance < 1000f)
        {
            // Show in meters with one decimal place
            return $"{distance:F1}m";
        }
        else
        {
            // Show in kilometers for large distances
            return $"{(distance / 1000f):F2}km";
        }
    }
    
    // Event handlers
    private void OnPowerChanged(float newPower)
    {
        UpdatePowerDisplay();
    }
    
    private void OnSpeedChanged(float newSpeed)
    {
        UpdateSpeedDisplay();
    }
    
    private void OnLightSpeedReached()
    {
        UpdateStatusDisplay();
        
        // Add special effects for light speed reached
        if (lightSpeedIndicator != null)
        {
            // Could add animation or effects here
            Debug.Log("Light speed indicator activated!");
        }
    }
    
    private void OnPowerCollected(float amount)
    {
        // Could add collection feedback effects here
        Debug.Log($"Power collected: {amount}");
    }
    
    private void OnComboChanged(int newCombo)
    {
        UpdateComboDisplay();
    }
    
    // Public methods for external control
    public void SetPowerBarVisible(bool visible)
    {
        if (powerBarFill != null)
        {
            powerBarFill.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    public void SetSpeedBarVisible(bool visible)
    {
        if (speedBarFill != null)
        {
            speedBarFill.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    public void SetComboVisible(bool visible)
    {
        if (comboContainer != null)
        {
            comboContainer.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    // Public methods for distance indicator control
    public void SetTargetDestination(Transform target)
    {
        targetDestination = target;
    }
    
    public void SetDistanceMode(bool showDestination, bool showNearestPowerOrb)
    {
        showDistanceToDestination = showDestination;
        showDistanceToNearestPowerOrb = showNearestPowerOrb;
    }
    
    public void SetDistanceIndicatorVisible(bool visible)
    {
        if (worldConnectionIndicator != null)
        {
            worldConnectionIndicator.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    public float GetCurrentDistance()
    {
        if (showDistanceToDestination && targetDestination != null)
        {
            return CalculateDistanceToTarget(targetDestination);
        }
        else if (showDistanceToNearestPowerOrb)
        {
            Transform nearestOrb = FindNearestPowerOrb();
            if (nearestOrb != null)
            {
                return CalculateDistanceToTarget(nearestOrb);
            }
        }
        return 0f;
    }
}
